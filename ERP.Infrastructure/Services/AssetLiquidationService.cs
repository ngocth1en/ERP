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
    public class AssetLiquidationService : IAssetLiquidationService
    {
        private readonly AppDbContext _context;
        private readonly ISequenceGenerator _sequenceGenerator;

        public AssetLiquidationService(AppDbContext context, ISequenceGenerator sequenceGenerator)
        {
            _context = context;
            _sequenceGenerator = sequenceGenerator;
        }

        // ─── TẠO PHIẾU ────────────────────────────────────────────

        public async Task<LiquidationOrderDto> CreateAsync(CreateLiquidationDto dto, string username)
        {
            if (!dto.AssetNos.Any())
                throw new Exception("Phiếu thanh lý phải có ít nhất 1 tài sản");

            var userId = await GetUserIdAsync(username);
            var id = await GenerateLiquidationIdAsync();

            var assets = await _context.Assets_Information
                .AsNoTracking()
                .Where(a => dto.AssetNos.Contains(a.AssetNo))
                .ToListAsync();

            var notFound = dto.AssetNos.Except(assets.Select(a => a.AssetNo)).ToList();
            if (notFound.Any())
                throw new Exception($"Tài sản không tồn tại: {string.Join(", ", notFound)}");

            var notScrapped = assets.Where(a => !a.Is_Deleted).ToList();
            if (notScrapped.Any())
                throw new Exception($"Tài sản chưa được báo phế: {string.Join(", ", notScrapped.Select(a => a.AssetNo))}");

            // Lấy ScrapId gần nhất tương ứng từng tài sản (nếu có)
            var scrapMap = await _context.Asset_Scrap_Details
                .AsNoTracking()
                .Where(d => dto.AssetNos.Contains(d.AssetNo) && d.Scrap.Status == ScrapStatus.Confirmed)
                .GroupBy(d => d.AssetNo)
                .Select(g => g.OrderByDescending(d => d.Id).First())
                .ToListAsync();

            var liquidation = new Asset_Liquidations
            {
                Id = id,
                DepartmentId = dto.DepartmentId,
                Status = LiquidationStatus.Pending,
                LiquidationMethod = dto.LiquidationMethod,
                RecoveredAmount = dto.RecoveredAmount,
                Note = dto.Note,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            _context.Asset_Liquidations.Add(liquidation);

            foreach (var asset in assets)
            {
                var scrapDetail = scrapMap.FirstOrDefault(d => d.AssetNo == asset.AssetNo);
                _context.Asset_Liquidation_Details.Add(new Asset_Liquidation_Details
                {
                    LiquidationId = id,
                    AssetNo = asset.AssetNo,
                    ScrapId = scrapDetail?.ScrapId,
                    PreviousUseDepartmentId = asset.UseDepartmentId
                });
            }

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── SỬA PHIẾU ──────────────────────────────────────────────

        public async Task<LiquidationOrderDto> UpdateAsync(string id, UpdateLiquidationDto dto, string username)
        {
            var liq = await GetLiquidationOrThrowAsync(id);

            if (liq.Status != LiquidationStatus.Draft && liq.Status != LiquidationStatus.Pending)
                throw new Exception("Chỉ có thể sửa phiếu ở trạng thái Nháp hoặc Chờ duyệt");

            var userId = await GetUserIdAsync(username);

            if (dto.LiquidationMethod != null) liq.LiquidationMethod = dto.LiquidationMethod;
            if (dto.RecoveredAmount.HasValue) liq.RecoveredAmount = dto.RecoveredAmount;
            if (dto.Note != null) liq.Note = dto.Note;

            liq.UpdatedAt = DateTime.Now;
            liq.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── XÁC NHẬN ─────────────────────────────────────────────

        public async Task<LiquidationOrderDto> ConfirmAsync(string id, string username)
        {
            var liq = await _context.Asset_Liquidations
                .Include(l => l.Details)
                .FirstOrDefaultAsync(l => l.Id == id)
                ?? throw new Exception("Phiếu thanh lý không tồn tại");

            if (liq.Status != LiquidationStatus.Pending)
                throw new Exception("Chỉ có thể xác nhận phiếu ở trạng thái Chờ duyệt");

            if (!liq.Details.Any())
                throw new Exception("Phiếu không có tài sản nào");

            var userId = await GetUserIdAsync(username);

            liq.Status = LiquidationStatus.Confirmed;
            liq.ConfirmedAt = DateTime.Now;
            liq.ConfirmedBy = userId;
            liq.UpdatedAt = DateTime.Now;
            liq.UpdatedBy = userId;

            // Tài sản giữ nguyên Is_Deleted = true (đã báo phế), thanh lý là bước xử lý cuối cùng
            // không cần thay đổi thêm dữ liệu Assets_Information

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── HỦY ────────────────────────────────────────────────────

        public async Task<LiquidationOrderDto> CancelAsync(string id, string username)
        {
            var liq = await GetLiquidationOrThrowAsync(id);

            if (liq.Status == LiquidationStatus.Confirmed)
                throw new Exception("Không thể hủy phiếu đã hoàn thành");

            if (liq.Status == LiquidationStatus.Cancelled)
                throw new Exception("Phiếu đã bị hủy trước đó");

            var userId = await GetUserIdAsync(username);

            liq.Status = LiquidationStatus.Cancelled;
            liq.UpdatedAt = DateTime.Now;
            liq.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── THÊM TÀI SẢN ───────────────────────────────────────────

        public async Task<LiquidationOrderDto> AddAssetsAsync(string id, AddLiquidationAssetsDto dto, string username)
        {
            var liq = await _context.Asset_Liquidations
                .Include(l => l.Details)
                .FirstOrDefaultAsync(l => l.Id == id)
                ?? throw new Exception("Phiếu thanh lý không tồn tại");

            if (liq.Status != LiquidationStatus.Draft && liq.Status != LiquidationStatus.Pending)
                throw new Exception("Chỉ có thể thêm tài sản khi phiếu ở trạng thái Nháp hoặc Chờ duyệt");

            if (!dto.AssetNos.Any())
                throw new Exception("Danh sách tài sản không được rỗng");

            var userId = await GetUserIdAsync(username);

            var existingAssetNos = liq.Details.Select(d => d.AssetNo).ToHashSet();
            var newAssetNos = dto.AssetNos.Where(n => !existingAssetNos.Contains(n)).ToList();

            if (!newAssetNos.Any())
                throw new Exception("Tất cả tài sản đã có trong phiếu");

            var assets = await _context.Assets_Information
                .AsNoTracking()
                .Where(a => newAssetNos.Contains(a.AssetNo) && a.Is_Deleted)
                .ToListAsync();

            var scrapMap = await _context.Asset_Scrap_Details
                .AsNoTracking()
                .Where(d => newAssetNos.Contains(d.AssetNo) && d.Scrap.Status == ScrapStatus.Confirmed)
                .GroupBy(d => d.AssetNo)
                .Select(g => g.OrderByDescending(d => d.Id).First())
                .ToListAsync();

            foreach (var asset in assets)
            {
                var scrapDetail = scrapMap.FirstOrDefault(d => d.AssetNo == asset.AssetNo);
                _context.Asset_Liquidation_Details.Add(new Asset_Liquidation_Details
                {
                    LiquidationId = id,
                    AssetNo = asset.AssetNo,
                    ScrapId = scrapDetail?.ScrapId,
                    PreviousUseDepartmentId = asset.UseDepartmentId
                });
            }

            liq.UpdatedAt = DateTime.Now;
            liq.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── XÓA TÀI SẢN ────────────────────────────────────────────

        public async Task RemoveAssetAsync(string id, int detailId, string username)
        {
            var liq = await GetLiquidationOrThrowAsync(id);

            if (liq.Status != LiquidationStatus.Draft && liq.Status != LiquidationStatus.Pending)
                throw new Exception("Chỉ có thể xóa tài sản khi phiếu ở trạng thái Nháp hoặc Chờ duyệt");

            var detail = await _context.Asset_Liquidation_Details
                .FirstOrDefaultAsync(d => d.Id == detailId && d.LiquidationId == id)
                ?? throw new Exception("Chi tiết không tồn tại trong phiếu");

            _context.Asset_Liquidation_Details.Remove(detail);
            await _context.SaveChangesAsync();
        }

        // ─── TÌM TÀI SẢN ĐÃ BÁO PHẾ ─────────────────────────────────

        public async Task<PagedResult<ScrappedAssetCandidateDto>> SearchScrappedAssetsAsync(SearchScrappedAssetsDto dto)
        {
            var processingAssetNos = await _context.Asset_Liquidation_Details
                .Where(d => d.Liquidation.Status == LiquidationStatus.Draft || d.Liquidation.Status == LiquidationStatus.Pending)
                .Select(d => d.AssetNo)
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

        public async Task<LiquidationOrderDto> GetAsync(string id)
        {
            var l = await _context.Asset_Liquidations
                .AsNoTracking()
                .Include(l => l.Department)
                .Include(l => l.Details)
                    .ThenInclude(d => d.Asset)
                        .ThenInclude(a => a.Model)
                            .ThenInclude(m => m.Small_Type)
                                .ThenInclude(st => st.Middle_Type)
                                    .ThenInclude(m => m.Big_Type)
                .FirstOrDefaultAsync(l => l.Id == id)
                ?? throw new Exception("Phiếu thanh lý không tồn tại");

            var createdBy = await GetUsernameByIdAsync(l.CreatedBy);
            var confirmedBy = l.ConfirmedBy.HasValue
                ? await GetUsernameByIdAsync(l.ConfirmedBy.Value)
                : null;

            return new LiquidationOrderDto
            {
                Id = l.Id,
                DepartmentId = l.DepartmentId,
                DepartmentName = l.Department.Name,
                Status = l.Status,
                LiquidationMethod = l.LiquidationMethod,
                RecoveredAmount = l.RecoveredAmount,
                Note = l.Note,
                CreatedAt = l.CreatedAt,
                CreatedByUserName = createdBy,
                ConfirmedAt = l.ConfirmedAt,
                ConfirmedByUserName = confirmedBy,
                Details = l.Details.Select(d => new LiquidationDetailDto
                {
                    Id = d.Id,
                    AssetNo = d.AssetNo,
                    AssetName = d.Asset.Model.Small_Type.Middle_Type.Big_Type.Big_Type_Name_VN + " - " +
                        d.Asset.Model.Small_Type.Middle_Type.Middle_Type_Name_VN + " - " +
                        d.Asset.Model.Small_Type.Small_Type_Name_VN,
                    ModelName = d.Asset.Model.Model_Name,
                    ScrapId = d.ScrapId,
                    PreviousUseDepartmentId = d.PreviousUseDepartmentId,
                    Note = d.Note
                }).ToList()
            };
        }

        // ─── SEARCH ─────────────────────────────────────────────────

        public async Task<PagedResult<LiquidationOrderListDto>> SearchAsync(SearchLiquidationOrdersDto dto)
        {
            var query = _context.Asset_Liquidations
                .AsNoTracking()
                .Include(l => l.Department)
                .Include(l => l.Details)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.DepartmentId))
                query = query.Where(l => l.DepartmentId == dto.DepartmentId);

            if (dto.Status.HasValue)
                query = query.Where(l => l.Status == dto.Status.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(l => new LiquidationOrderListDto
                {
                    Id = l.Id,
                    DepartmentId = l.DepartmentId,
                    DepartmentName = l.Department.Name,
                    Status = l.Status,
                    AssetCount = l.Details.Count,
                    CreatedAt = l.CreatedAt,
                    CreatedByUserName = _context.Users
                        .Where(u => u.Id == l.CreatedBy)
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return new PagedResult<LiquidationOrderListDto> { Total = total, Items = items };
        }

        // ─── HELPERS ────────────────────────────────────────────────

        private async Task<Asset_Liquidations> GetLiquidationOrThrowAsync(string id) =>
            await _context.Asset_Liquidations.FirstOrDefaultAsync(l => l.Id == id)
            ?? throw new Exception("Phiếu thanh lý không tồn tại");

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

        //private async Task<string> GenerateLiquidationIdAsync()
        //{
        //    var prefix = "LQ" + DateTime.Now.ToString("yyyyMM");
        //    var last = await _context.Asset_Liquidations
        //        .AsNoTracking()
        //        .Where(l => l.Id.StartsWith(prefix))
        //        .OrderByDescending(l => l.Id)
        //        .Select(l => l.Id)
        //        .FirstOrDefaultAsync();

        //    int next = 1;
        //    if (last != null && int.TryParse(last[prefix.Length..], out int parsed))
        //        next = parsed + 1;

        //    return prefix + next.ToString("D4");
        //}
        private Task<string> GenerateLiquidationIdAsync() =>
    _sequenceGenerator.NextAsync("LQ" + DateTime.Now.ToString("yyyyMM"));
    }
}
