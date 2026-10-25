using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Application.Common;
using ERP.Domain.Entities.ASM;
using ERP.Domain.Enum.ASM;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Services
{
    public class AssetRepairService : IAssetRepairService
    {
        private readonly AppDbContext _context;
        private readonly ISequenceGenerator _sequenceGenerator;

        public AssetRepairService(AppDbContext context, ISequenceGenerator sequenceGenerator)
        {
            _context = context;
            _sequenceGenerator = sequenceGenerator;
        }

        // ─── TẠO ĐƠN ────────────────────────────────────────────────

        public async Task<RepairOrderDto> CreateAsync(CreateRepairDto dto, string username)
        {
            var asset = await _context.Assets_Information
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AssetNo == dto.AssetNo)
                ?? throw new Exception("Tài sản không tồn tại");

            if (!asset.Is_Deleted)
                throw new Exception("Chỉ có thể tạo đơn sửa chữa cho tài sản đã báo phế");

            // Nếu có ScrapId, validate phiếu báo phế đã Confirmed
            if (!string.IsNullOrEmpty(dto.ScrapId))
            {
                var scrap = await _context.Asset_Scraps
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == dto.ScrapId)
                    ?? throw new Exception("Phiếu báo phế không tồn tại");

                if (scrap.Status != ScrapStatus.Confirmed)
                    throw new Exception("Phiếu báo phế phải ở trạng thái Đã báo phế");
            }

            // Không cho tạo trùng đơn đang xử lý cho cùng 1 tài sản
            var existing = await _context.Asset_Repairs
                .AsNoTracking()
                .AnyAsync(r => r.AssetNo == dto.AssetNo &&
                    (r.Status == RepairStatus.Draft || r.Status == RepairStatus.Pending || r.Status == RepairStatus.InProgress));
            if (existing)
                throw new Exception("Tài sản đã có đơn sửa chữa đang xử lý");

            var userId = await GetUserIdAsync(username);
            var id = await GenerateRepairIdAsync();

            var repair = new Asset_Repairs
            {
                Id = id,
                DepartmentId = asset.UseDepartmentId,
                AssetNo = dto.AssetNo,
                ScrapId = dto.ScrapId,
                RepairVendor = dto.RepairVendor,
                Status = RepairStatus.Pending,
                Reason = dto.Reason,
                Cost = dto.Cost,
                Note = dto.Note,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            _context.Asset_Repairs.Add(repair);
            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── SỬA ĐƠN ────────────────────────────────────────────────

        public async Task<RepairOrderDto> UpdateAsync(string id, UpdateRepairDto dto, string username)
        {
            var repair = await GetRepairOrThrowAsync(id);

            if (repair.Status != RepairStatus.Draft && repair.Status != RepairStatus.Pending)
                throw new Exception("Chỉ có thể sửa đơn ở trạng thái Nháp hoặc Chờ duyệt");

            var userId = await GetUserIdAsync(username);

            if (dto.RepairVendor != null) repair.RepairVendor = dto.RepairVendor;
            if (dto.Reason != null) repair.Reason = dto.Reason;
            if (dto.Cost.HasValue) repair.Cost = dto.Cost;
            if (dto.Note != null) repair.Note = dto.Note;

            repair.UpdatedAt = DateTime.Now;
            repair.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── BẮT ĐẦU SỬA CHỮA ───────────────────────────────────────

        public async Task<RepairOrderDto> StartAsync(string id, string username)
        {
            var repair = await GetRepairOrThrowAsync(id);

            if (repair.Status != RepairStatus.Pending)
                throw new Exception("Chỉ có thể bắt đầu sửa chữa từ trạng thái Chờ duyệt");

            var userId = await GetUserIdAsync(username);

            repair.Status = RepairStatus.InProgress;
            repair.UpdatedAt = DateTime.Now;
            repair.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── HOÀN THÀNH SỬA CHỮA ────────────────────────────────────

        public async Task<RepairOrderDto> CompleteAsync(string id, string username)
        {
            var repair = await GetRepairOrThrowAsync(id);

            if (repair.Status != RepairStatus.InProgress)
                throw new Exception("Chỉ có thể hoàn thành đơn đang ở trạng thái Đang sửa chữa");

            var userId = await GetUserIdAsync(username);

            var asset = await _context.Assets_Information
                .FirstOrDefaultAsync(a => a.AssetNo == repair.AssetNo)
                ?? throw new Exception("Tài sản không tồn tại");

            // Phục hồi tài sản để có thể sử dụng lại
            asset.Is_Deleted = false;
            asset.UpdatedAt = DateTime.Now;
            asset.UpdatedBy = userId;

            repair.Status = RepairStatus.Completed;
            repair.CompletedAt = DateTime.Now;
            repair.CompletedBy = userId;
            repair.UpdatedAt = DateTime.Now;
            repair.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── HỦY ────────────────────────────────────────────────────

        public async Task<RepairOrderDto> CancelAsync(string id, string username)
        {
            var repair = await GetRepairOrThrowAsync(id);

            if (repair.Status == RepairStatus.Completed)
                throw new Exception("Không thể hủy đơn đã hoàn thành");

            if (repair.Status == RepairStatus.Cancelled)
                throw new Exception("Đơn đã bị hủy trước đó");

            var userId = await GetUserIdAsync(username);

            repair.Status = RepairStatus.Cancelled;
            repair.UpdatedAt = DateTime.Now;
            repair.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── TÌM TÀI SẢN ĐÃ BÁO PHẾ ─────────────────────────────────

        public async Task<PagedResult<ScrappedAssetCandidateDto>> SearchScrappedAssetsAsync(SearchScrappedAssetsDto dto)
        {
            // Tài sản Is_Deleted = true và có phiếu báo phế Confirmed, chưa có đơn xử lý (repair/liquidation) đang chạy
            var processingAssetNos = await _context.Asset_Repairs
                .Where(r => r.Status == RepairStatus.Draft || r.Status == RepairStatus.Pending || r.Status == RepairStatus.InProgress)
                .Select(r => r.AssetNo)
                .ToListAsync();

            var query = _context.Asset_Scrap_Details
                .AsNoTracking()
                .Include(d => d.Scrap)
                .Include(d => d.Asset)
                    .ThenInclude(a => a.Model)
                        .ThenInclude(m => m.Small_Type)
                            .ThenInclude(s => s.Middle_Type)
                                .ThenInclude(m => m.Big_Type)
                .Where(d => d.Scrap.Status == ScrapStatus.Confirmed
                    && d.Asset.Is_Deleted
                    && !processingAssetNos.Contains(d.AssetNo));

            if (!string.IsNullOrEmpty(dto.DepartmentId))
                query = query.Where(d => d.Scrap.DepartmentId == dto.DepartmentId);

            if (!string.IsNullOrEmpty(dto.AssetNo))
                query = query.Where(d => d.AssetNo.Contains(dto.AssetNo));

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(d => d.Scrap.ConfirmedAt)
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(d => new ScrappedAssetCandidateDto
                {
                    AssetNo = d.AssetNo,
                    AssetName = d.Asset.Model.Big_Type.Big_Type_Name_VN + " - " +
                                d.Asset.Model.Middle_Type.Middle_Type_Name_VN + " - " +
                                d.Asset.Model.Small_Type.Small_Type_Name_VN,
                    ModelName = d.Asset.Model.Model_Name,
                    ScrapId = d.ScrapId,
                    DepartmentId = d.Scrap.DepartmentId,
                    ScrapConfirmedAt = d.Scrap.ConfirmedAt!.Value
                })
                .ToListAsync();

            return new PagedResult<ScrappedAssetCandidateDto> { Total = total, Items = items };
        }

        // ─── GET ────────────────────────────────────────────────────

        public async Task<RepairOrderDto> GetAsync(string id)
        {
            var r = await _context.Asset_Repairs
                .AsNoTracking()
                .Include(r => r.Department)
                .Include(r => r.Asset)
                    .ThenInclude(a => a.Model)
                        .ThenInclude(m => m.Small_Type)
                            .ThenInclude(s => s.Middle_Type)
                                .ThenInclude(m => m.Big_Type)
                .FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new Exception("Đơn sửa chữa không tồn tại");

            var createdBy = await GetUsernameByIdAsync(r.CreatedBy);
            var completedBy = r.CompletedBy.HasValue
                ? await GetUsernameByIdAsync(r.CompletedBy.Value)
                : null;

            return new RepairOrderDto
            {
                Id = r.Id,
                DepartmentId = r.DepartmentId,
                DepartmentName = r.Department.Name,
                AssetNo = r.AssetNo,
                AssetName = r.Asset.Model.Big_Type.Big_Type_Name_VN + " - " +
                            r.Asset.Model.Middle_Type.Middle_Type_Name_VN + " - " +
                            r.Asset.Model.Small_Type.Small_Type_Name_VN,
                ModelName = r.Asset.Model.Model_Name,
                ScrapId = r.ScrapId,
                RepairVendor = r.RepairVendor,
                Status = r.Status,
                Reason = r.Reason,
                Cost = r.Cost,
                Note = r.Note,
                CreatedAt = r.CreatedAt,
                CreatedByUserName = createdBy,
                CompletedAt = r.CompletedAt,
                CompletedByUserName = completedBy
            };
        }

        // ─── SEARCH ─────────────────────────────────────────────────

        public async Task<PagedResult<RepairOrderListDto>> SearchAsync(SearchRepairOrdersDto dto)
        {
            var query = _context.Asset_Repairs
                .AsNoTracking()
                .Include(r => r.Department)
                .Include(r => r.Asset)
                    .ThenInclude(a => a.Model)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.DepartmentId))
                query = query.Where(r => r.DepartmentId == dto.DepartmentId);

            if (!string.IsNullOrEmpty(dto.AssetNo))
                query = query.Where(r => r.AssetNo.Contains(dto.AssetNo));

            if (dto.Status.HasValue)
                query = query.Where(r => r.Status == dto.Status.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(r => new RepairOrderListDto
                {
                    Id = r.Id,
                    DepartmentId = r.DepartmentId,
                    DepartmentName = r.Department.Name,
                    AssetNo = r.AssetNo,
                    AssetName = r.Asset.Model.Model_Name,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    CreatedByUserName = _context.Users
                        .Where(u => u.Id == r.CreatedBy)
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return new PagedResult<RepairOrderListDto> { Total = total, Items = items };
        }

        // ─── HELPERS ────────────────────────────────────────────────

        private async Task<Asset_Repairs> GetRepairOrThrowAsync(string id) =>
            await _context.Asset_Repairs.FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new Exception("Đơn sửa chữa không tồn tại");

        private async Task<Guid> GetUserIdAsync(string username)
        {
            if (string.IsNullOrEmpty(username)) return Guid.Empty;
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username)
                ?? throw new Exception($"User [{username}] không tồn tại");
            return user.Id;
        }

        private async Task<string?> GetUsernameByIdAsync(Guid userId) =>
            await _context.Users.AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

        //private async Task<string> GenerateRepairIdAsync()
        //{
        //    var prefix = "RP" + DateTime.Now.ToString("yyyyMM");
        //    var last = await _context.Asset_Repairs
        //        .AsNoTracking()
        //        .Where(r => r.Id.StartsWith(prefix))
        //        .OrderByDescending(r => r.Id)
        //        .Select(r => r.Id)
        //        .FirstOrDefaultAsync();

        //    int next = 1;
        //    if (last != null && int.TryParse(last[prefix.Length..], out int parsed))
        //        next = parsed + 1;

        //    return prefix + next.ToString("D4");
        //}
        private Task<string> GenerateRepairIdAsync() =>
    _sequenceGenerator.NextAsync("RP" + DateTime.Now.ToString("yyyyMM"));
        // ─── LỊCH SỬ SỬA CHỮA (BẢO TRÌ MÁY) ─────────────────────────

        public async Task<PagedResult<RepairHistoryItemDto>> GetMaintenanceHistoryAsync(MaintenanceHistoryQueryDto dto)
        {
            var query = _context.Asset_Repairs
                .AsNoTracking()
                .Include(r => r.Department)
                .Include(r => r.Asset)
                    .ThenInclude(a => a.Model)
                        .ThenInclude(m => m.Small_Type)
                            .ThenInclude(s => s.Middle_Type)
                                .ThenInclude(m => m.Big_Type)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.AssetNo))
                query = query.Where(r => r.AssetNo == dto.AssetNo);

            if (!string.IsNullOrEmpty(dto.DepartmentId))
                query = query.Where(r => r.DepartmentId == dto.DepartmentId);

            if (dto.Status.HasValue)
                query = query.Where(r => r.Status == dto.Status.Value);

            if (dto.FromDate.HasValue)
                query = query.Where(r => r.CreatedAt >= dto.FromDate.Value);

            if (dto.ToDate.HasValue)
                query = query.Where(r => r.CreatedAt <= dto.ToDate.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(r => new RepairHistoryItemDto
                {
                    Id = r.Id,
                    AssetNo = r.AssetNo,
                    AssetName = r.Asset.Model.Big_Type.Big_Type_Name_VN + " - " +
                                r.Asset.Model.Middle_Type.Middle_Type_Name_VN + " - " +
                                r.Asset.Model.Small_Type.Small_Type_Name_VN,
                    DepartmentId = r.DepartmentId,
                    DepartmentName = r.Department.Name,
                    RepairVendor = r.RepairVendor,
                    Reason = r.Reason,
                    Cost = r.Cost,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    CreatedByUserName = _context.Users
                        .Where(u => u.Id == r.CreatedBy)
                        .Select(u => u.UserName)
                        .FirstOrDefault(),
                    CompletedAt = r.CompletedAt,
                    CompletedByUserName = r.CompletedBy == null ? null : _context.Users
                        .Where(u => u.Id == r.CompletedBy)
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return new PagedResult<RepairHistoryItemDto> { Total = total, Items = items };
        }

        // ─── THỐNG KÊ CHI PHÍ THEO TÀI SẢN ───────────────────────────

        public async Task<PagedResult<AssetMaintenanceSummaryDto>> GetMaintenanceSummaryByAssetAsync(MaintenanceHistoryQueryDto dto)
        {
            var query = _context.Asset_Repairs
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.AssetNo))
                query = query.Where(r => r.AssetNo == dto.AssetNo);

            if (!string.IsNullOrEmpty(dto.DepartmentId))
                query = query.Where(r => r.DepartmentId == dto.DepartmentId);

            if (dto.FromDate.HasValue)
                query = query.Where(r => r.CreatedAt >= dto.FromDate.Value);

            if (dto.ToDate.HasValue)
                query = query.Where(r => r.CreatedAt <= dto.ToDate.Value);

            var grouped = query
                .GroupBy(r => r.AssetNo)
                .Select(g => new
                {
                    AssetNo = g.Key,
                    DepartmentId = g.OrderByDescending(x => x.CreatedAt).Select(x => x.DepartmentId).First(),
                    TotalRepairCount = g.Count(),
                    CompletedRepairCount = g.Count(x => x.Status == RepairStatus.Completed),
                    TotalCost = g.Sum(x => x.Cost ?? 0),
                    LastRepairAt = g.Max(x => x.CreatedAt)
                });

            var total = await grouped.CountAsync();

            var pageItems = await grouped
                .OrderByDescending(x => x.TotalCost)
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();

            if (pageItems.Count == 0)
                return new PagedResult<AssetMaintenanceSummaryDto> { Total = total, Items = new() };

            // ✅ Gộp thành 1 truy vấn duy nhất thay vì N+1 query trong vòng lặp
            var assetNos = pageItems.Select(p => p.AssetNo).ToList();
            var departmentIds = pageItems.Select(p => p.DepartmentId).Distinct().ToList();

            var assets = await _context.Assets_Information
                .AsNoTracking()
                .Include(a => a.Model)
                    .ThenInclude(m => m.Small_Type)
                        .ThenInclude(s => s.Middle_Type)
                            .ThenInclude(m => m.Big_Type)
                .Where(a => assetNos.Contains(a.AssetNo))
                .ToDictionaryAsync(a => a.AssetNo);

            var departments = await _context.Departments
                .AsNoTracking()
                .Where(d => departmentIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name);

            var items = pageItems.Select(g =>
            {
                assets.TryGetValue(g.AssetNo, out var asset);
                departments.TryGetValue(g.DepartmentId, out var departmentName);

                return new AssetMaintenanceSummaryDto
                {
                    AssetNo = g.AssetNo,
                    AssetName = asset == null ? null :
                        asset.Model.Big_Type.Big_Type_Name_VN + " - " +
                        asset.Model.Middle_Type.Middle_Type_Name_VN + " - " +
                        asset.Model.Small_Type.Small_Type_Name_VN,
                    ModelName = asset?.Model.Model_Name,
                    DepartmentId = g.DepartmentId,
                    DepartmentName = departmentName ?? string.Empty,
                    TotalRepairCount = g.TotalRepairCount,
                    CompletedRepairCount = g.CompletedRepairCount,
                    TotalCost = g.TotalCost,
                    LastRepairAt = g.LastRepairAt
                };
            }).ToList();

            return new PagedResult<AssetMaintenanceSummaryDto> { Total = total, Items = items };
        }

        // ─── THỐNG KÊ TỔNG HỢP (DASHBOARD) ───────────────────────────

        public async Task<MaintenanceOverallStatisticDto> GetOverallStatisticAsync(DateTime? fromDate, DateTime? toDate, int topCount = 10)
        {
            var query = _context.Asset_Repairs.AsNoTracking().AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(r => r.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.CreatedAt <= toDate.Value);

            var totalRepairOrders = await query.CountAsync();
            var completedRepairOrders = await query.CountAsync(r => r.Status == RepairStatus.Completed);
            var inProgressRepairOrders = await query.CountAsync(r => r.Status == RepairStatus.InProgress);
            var totalCost = await query.SumAsync(r => r.Cost ?? 0);

            var topAssets = await GetMaintenanceSummaryByAssetAsync(new MaintenanceHistoryQueryDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                PageIndex = 1,
                PageSize = topCount
            });

            return new MaintenanceOverallStatisticDto
            {
                TotalRepairOrders = totalRepairOrders,
                CompletedRepairOrders = completedRepairOrders,
                InProgressRepairOrders = inProgressRepairOrders,
                TotalCost = totalCost,
                TopAssetsByCost = topAssets.Items
            };
        }
    }
}
