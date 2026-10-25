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
    public class AssetScrapService : IAssetScrapService
    {
        private readonly AppDbContext _context;
        private readonly ISequenceGenerator _sequenceGenerator;

        public AssetScrapService(AppDbContext context, ISequenceGenerator sequenceGenerator)
        {
            _context = context;
            _sequenceGenerator = sequenceGenerator;
        }

        // ─── TẠO PHIẾU ────────────────────────────────────────────

        public async Task<ScrapOrderDto> CreateAsync(CreateScrapDto dto, string username)
        {
            if (!dto.AssetNos.Any())
                throw new Exception("Phiếu báo phế phải có ít nhất 1 tài sản");

            var userId = await GetUserIdAsync(username);
            var id = await GenerateScrapIdAsync();

            // Validate tài sản: phải thuộc đơn vị báo phế và không bị xóa
            var assets = await _context.Assets_Information
                .AsNoTracking()
                .Where(a => dto.AssetNos.Contains(a.AssetNo) && !a.Is_Deleted)
                .ToListAsync();

            var notFound = dto.AssetNos.Except(assets.Select(a => a.AssetNo)).ToList();
            if (notFound.Any())
                throw new Exception($"Tài sản không tồn tại: {string.Join(", ", notFound)}");

            var notOwned = assets.Where(a => a.UseDepartmentId != dto.DepartmentId).ToList();
            if (notOwned.Any())
                throw new Exception($"Tài sản không thuộc đơn vị báo phế: {string.Join(", ", notOwned.Select(a => a.AssetNo))}");

            var scrap = new Asset_Scraps
            {
                Id = id,
                DepartmentId = dto.DepartmentId,
                StorageLocationNo = dto.StorageLocationNo,
                Status = ScrapStatus.Pending,
                Reason = dto.Reason,
                Note = dto.Note,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            _context.Asset_Scraps.Add(scrap);

            foreach (var asset in assets)
            {
                _context.Asset_Scrap_Details.Add(new Asset_Scrap_Details
                {
                    ScrapId = id,
                    AssetNo = asset.AssetNo,
                    PreviousUseDepartmentId = asset.UseDepartmentId,
                    PreviousStorageLocationNo = asset.StorageLocationNo
                });
            }

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── SỬA PHIẾU (Draft / Pending) ─────────────────────────

        public async Task<ScrapOrderDto> UpdateAsync(string id, UpdateScrapDto dto, string username)
        {
            var scrap = await GetScrapOrThrowAsync(id);

            if (scrap.Status != ScrapStatus.Draft && scrap.Status != ScrapStatus.Pending)
                throw new Exception("Chỉ có thể sửa phiếu ở trạng thái Nháp hoặc Chờ duyệt");

            var userId = await GetUserIdAsync(username);

            if (dto.Reason != null) scrap.Reason = dto.Reason;
            if (dto.Note != null) scrap.Note = dto.Note;

            scrap.UpdatedAt = DateTime.Now;
            scrap.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── XÁC NHẬN ─────────────────────────────────────────────

        public async Task<ScrapOrderDto> ConfirmAsync(string id, string username)
        {
            var scrap = await _context.Asset_Scraps
                .Include(s => s.Details)
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new Exception("Phiếu báo phế không tồn tại");

            if (scrap.Status != ScrapStatus.Pending)
                throw new Exception("Chỉ có thể xác nhận phiếu ở trạng thái Chờ duyệt");

            if (!scrap.Details.Any())
                throw new Exception("Phiếu không có tài sản nào");

            var userId = await GetUserIdAsync(username);

            // Cập nhật tài sản: đánh dấu đã báo phế (xóa khỏi hệ thống)
            var assetNos = scrap.Details.Select(d => d.AssetNo).ToList();
            var assets = await _context.Assets_Information
                .Where(a => assetNos.Contains(a.AssetNo))
                .ToListAsync();

            foreach (var asset in assets)
            {
                asset.Is_Deleted = true;
                asset.UpdatedAt = DateTime.Now;
                asset.UpdatedBy = userId;
            }

            scrap.Status = ScrapStatus.Confirmed;
            scrap.ConfirmedAt = DateTime.Now;
            scrap.ConfirmedBy = userId;
            scrap.UpdatedAt = DateTime.Now;
            scrap.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── HỦY ───────────────────────────────────────────────────

        public async Task<ScrapOrderDto> CancelAsync(string id, string username)
        {
            var scrap = await GetScrapOrThrowAsync(id);

            if (scrap.Status == ScrapStatus.Confirmed)
                throw new Exception("Không thể hủy phiếu đã hoàn thành");

            if (scrap.Status == ScrapStatus.Cancelled)
                throw new Exception("Phiếu đã bị hủy trước đó");

            var userId = await GetUserIdAsync(username);

            scrap.Status = ScrapStatus.Cancelled;
            scrap.UpdatedAt = DateTime.Now;
            scrap.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── THÊM TÀI SẢN VÀO PHIẾU ─────────────────────────────

        public async Task<ScrapOrderDto> AddAssetsAsync(string id, AddScrapAssetsDto dto, string username)
        {
            var scrap = await _context.Asset_Scraps
                .Include(s => s.Details)
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new Exception("Phiếu báo phế không tồn tại");

            if (scrap.Status != ScrapStatus.Draft && scrap.Status != ScrapStatus.Pending)
                throw new Exception("Chỉ có thể thêm tài sản khi phiếu ở trạng thái Nháp hoặc Chờ duyệt");

            if (!dto.AssetNos.Any())
                throw new Exception("Danh sách tài sản không được rỗng");

            var userId = await GetUserIdAsync(username);

            // Lọc tài sản chưa có trong phiếu
            var existingAssetNos = scrap.Details.Select(d => d.AssetNo).ToHashSet();
            var newAssetNos = dto.AssetNos.Where(n => !existingAssetNos.Contains(n)).ToList();

            if (!newAssetNos.Any())
                throw new Exception("Tất cả tài sản đã có trong phiếu");

            var assets = await _context.Assets_Information
                .AsNoTracking()
                .Where(a => newAssetNos.Contains(a.AssetNo) && !a.Is_Deleted)
                .ToListAsync();

            var notOwned = assets.Where(a => a.UseDepartmentId != scrap.DepartmentId).ToList();
            if (notOwned.Any())
                throw new Exception($"Tài sản không thuộc đơn vị báo phế: {string.Join(", ", notOwned.Select(a => a.AssetNo))}");

            foreach (var asset in assets)
            {
                _context.Asset_Scrap_Details.Add(new Asset_Scrap_Details
                {
                    ScrapId = id,
                    AssetNo = asset.AssetNo,
                    PreviousUseDepartmentId = asset.UseDepartmentId,
                    PreviousStorageLocationNo = asset.StorageLocationNo
                });
            }

            scrap.UpdatedAt = DateTime.Now;
            scrap.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── XÓA TÀI SẢN KHỎI PHIẾU ─────────────────────────────

        public async Task RemoveAssetAsync(string id, int detailId, string username)
        {
            var scrap = await GetScrapOrThrowAsync(id);

            if (scrap.Status != ScrapStatus.Draft && scrap.Status != ScrapStatus.Pending)
                throw new Exception("Chỉ có thể xóa tài sản khi phiếu ở trạng thái Nháp hoặc Chờ duyệt");

            var detail = await _context.Asset_Scrap_Details
                .FirstOrDefaultAsync(d => d.Id == detailId && d.ScrapId == id)
                ?? throw new Exception("Chi tiết không tồn tại trong phiếu");

            _context.Asset_Scrap_Details.Remove(detail);
            await _context.SaveChangesAsync();
        }

        // ─── TÌM KIẾM TÀI SẢN CÓ THỂ BÁO PHẾ ──────────────────

        public async Task<PagedResult<ScrapAssetCandidateDto>> SearchCandidateAssetsAsync(SearchScrapAssetsDto dto)
        {
            var query = _context.Assets_Information
                .AsNoTracking()
                .Include(a => a.Model)
                    .ThenInclude(m => m.Small_Type)
                        .ThenInclude(s => s.Middle_Type)
                            .ThenInclude(m => m.Big_Type)
                .Where(a => !a.Is_Deleted && a.UseDepartmentId == dto.DepartmentId);

            if (!string.IsNullOrEmpty(dto.StorageLocationNo))
                query = query.Where(a => a.StorageLocationNo == dto.StorageLocationNo);

            if (!string.IsNullOrEmpty(dto.AssetNo))
                query = query.Where(a => a.AssetNo.Contains(dto.AssetNo));

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.AssetNo)
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(a => new ScrapAssetCandidateDto
                {
                    AssetNo = a.AssetNo,
                    AssetName = a.Model.Big_Type.Big_Type_Name_VN + " - " +
                                a.Model.Middle_Type.Middle_Type_Name_VN + " - " +
                                a.Model.Small_Type.Small_Type_Name_VN,
                    ModelName = a.Model.Model_Name,
                    StorageLocationNo = a.StorageLocationNo,
                    UseDepartmentId = a.UseDepartmentId,
                    Status = a.Status
                })
                .ToListAsync();

            return new PagedResult<ScrapAssetCandidateDto> { Total = total, Items = items };
        }

        // ─── GET ───────────────────────────────────────────────────

        public async Task<ScrapOrderDto> GetAsync(string id)
        {
            var s = await _context.Asset_Scraps
                .AsNoTracking()
                .Include(s => s.Department)
                .Include(s => s.Details)
                    .ThenInclude(d => d.Asset)
                        .ThenInclude(a => a.Model)
                            .ThenInclude(m => m.Small_Type)
                                .ThenInclude(st => st.Middle_Type)
                                    .ThenInclude(m => m.Big_Type)
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new Exception("Phiếu báo phế không tồn tại");

            var createdBy = await GetUsernameByIdAsync(s.CreatedBy);
            var confirmedBy = s.ConfirmedBy.HasValue
                ? await GetUsernameByIdAsync(s.ConfirmedBy.Value)
                : null;

            return new ScrapOrderDto
            {
                Id = s.Id,
                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department.Name,
                StorageLocationNo = s.StorageLocationNo,
                Status = s.Status,
                Reason = s.Reason,
                Note = s.Note,
                CreatedAt = s.CreatedAt,
                CreatedByUserName = createdBy,
                ConfirmedAt = s.ConfirmedAt,
                ConfirmedByUserName = confirmedBy,
                Details = s.Details.Select(d => new ScrapDetailDto
                {
                    Id = d.Id,
                    AssetNo = d.AssetNo,
                    AssetName = d.Asset.Model.Small_Type.Middle_Type.Big_Type.Big_Type_Name_VN + " - " +
                d.Asset.Model.Small_Type.Middle_Type.Middle_Type_Name_VN + " - " +
                d.Asset.Model.Small_Type.Small_Type_Name_VN,
                    ModelName = d.Asset.Model.Model_Name,
                    PreviousUseDepartmentId = d.PreviousUseDepartmentId,
                    PreviousStorageLocationNo = d.PreviousStorageLocationNo,
                    Note = d.Note
                }).ToList()
            };
        }

        // ─── SEARCH ────────────────────────────────────────────────

        public async Task<PagedResult<ScrapOrderListDto>> SearchAsync(SearchScrapOrdersDto dto)
        {
            var query = _context.Asset_Scraps
                .AsNoTracking()
                .Include(s => s.Department)
                .Include(s => s.Details)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.DepartmentId))
                query = query.Where(s => s.DepartmentId == dto.DepartmentId);

            if (dto.Status.HasValue)
                query = query.Where(s => s.Status == dto.Status.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(s => new ScrapOrderListDto
                {
                    Id = s.Id,
                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.Department.Name,
                    Status = s.Status,
                    AssetCount = s.Details.Count,
                    CreatedAt = s.CreatedAt,
                    CreatedByUserName = _context.Users
                        .Where(u => u.Id == s.CreatedBy)
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return new PagedResult<ScrapOrderListDto> { Total = total, Items = items };
        }

        // ─── HELPERS ───────────────────────────────────────────────

        private async Task<Asset_Scraps> GetScrapOrThrowAsync(string id) =>
            await _context.Asset_Scraps.FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new Exception("Phiếu báo phế không tồn tại");

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

        //private async Task<string> GenerateScrapIdAsync()
        //{
        //    var prefix = "SC" + DateTime.Now.ToString("yyyyMM");
        //    var last = await _context.Asset_Scraps
        //        .AsNoTracking()
        //        .Where(s => s.Id.StartsWith(prefix))
        //        .OrderByDescending(s => s.Id)
        //        .Select(s => s.Id)
        //        .FirstOrDefaultAsync();

        //    int next = 1;
        //    if (last != null && int.TryParse(last[prefix.Length..], out int parsed))
        //        next = parsed + 1;

        //    return prefix + next.ToString("D4");
        //}
        private Task<string> GenerateScrapIdAsync() =>
    _sequenceGenerator.NextAsync("SC" + DateTime.Now.ToString("yyyyMM"));
    }
}
