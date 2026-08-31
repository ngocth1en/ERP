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
    public class AssetHandoverService : IAssetHandoverService
    {
        private readonly AppDbContext _context;

        public AssetHandoverService(AppDbContext context)
        {
            _context = context;
        }

        // ─── TẠO PHIẾU ────────────────────────────────────────────

        public async Task<HandoverOrderDto> CreateAsync(CreateHandoverDto dto, string username)
        {
            if (dto.FromDepartmentId == dto.ToDepartmentId)
                throw new Exception("Đơn vị giao và đơn vị nhận không được trùng nhau");

            if (!dto.AssetNos.Any())
                throw new Exception("Phiếu bàn giao phải có ít nhất 1 tài sản");

            var userId = await GetUserIdAsync(username);
            var id = await GenerateHandoverIdAsync();

            // Validate tài sản: phải thuộc đơn vị giao và không bị xóa
            var assets = await _context.Assets_Information
                .AsNoTracking()
                .Where(a => dto.AssetNos.Contains(a.AssetNo) && !a.Is_Deleted)
                .ToListAsync();

            var notFound = dto.AssetNos.Except(assets.Select(a => a.AssetNo)).ToList();
            if (notFound.Any())
                throw new Exception($"Tài sản không tồn tại: {string.Join(", ", notFound)}");

            var notOwned = assets.Where(a => a.UseDepartmentId != dto.FromDepartmentId).ToList();
            if (notOwned.Any())
                throw new Exception($"Tài sản không thuộc đơn vị giao: {string.Join(", ", notOwned.Select(a => a.AssetNo))}");

            var handover = new Asset_Handovers
            {
                Id = id,
                FromDepartmentId = dto.FromDepartmentId,
                ToDepartmentId = dto.ToDepartmentId,
                FromStorageLocationNo = dto.FromStorageLocationNo,
                ToStorageLocationNo = dto.ToStorageLocationNo,
                Status = HandoverStatus.Pending,
                Note = dto.Note,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            _context.Asset_Handovers.Add(handover);

            foreach (var asset in assets)
            {
                _context.Asset_Handover_Details.Add(new Asset_Handover_Details
                {
                    HandoverId = id,
                    AssetNo = asset.AssetNo,
                    PreviousUseDepartmentId = asset.UseDepartmentId,
                    PreviousStorageLocationNo = asset.StorageLocationNo
                });
            }

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── SỬA PHIẾU (Draft / Pending) ─────────────────────────

        public async Task<HandoverOrderDto> UpdateAsync(string id, UpdateHandoverDto dto, string username)
        {
            var handover = await GetHandoverOrThrowAsync(id);

            if (handover.Status != HandoverStatus.Draft && handover.Status != HandoverStatus.Pending)
                throw new Exception("Chỉ có thể sửa phiếu ở trạng thái Nháp hoặc Chờ duyệt");

            var userId = await GetUserIdAsync(username);

            if (dto.ToDepartmentId != null)
            {
                if (dto.ToDepartmentId == handover.FromDepartmentId)
                    throw new Exception("Đơn vị nhận không được trùng đơn vị giao");
                handover.ToDepartmentId = dto.ToDepartmentId;
            }

            if (dto.FromStorageLocationNo != null) handover.FromStorageLocationNo = dto.FromStorageLocationNo;
            if (dto.ToStorageLocationNo != null) handover.ToStorageLocationNo = dto.ToStorageLocationNo;
            if (dto.Note != null) handover.Note = dto.Note;

            handover.UpdatedAt = DateTime.Now;
            handover.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── XÁC NHẬN ─────────────────────────────────────────────

        public async Task<HandoverOrderDto> ConfirmAsync(string id, string username)
        {
            var handover = await _context.Asset_Handovers
                .Include(h => h.Details)
                .FirstOrDefaultAsync(h => h.Id == id)
                ?? throw new Exception("Phiếu bàn giao không tồn tại");

            if (handover.Status != HandoverStatus.Pending)
                throw new Exception("Chỉ có thể xác nhận phiếu ở trạng thái Chờ duyệt");

            if (!handover.Details.Any())
                throw new Exception("Phiếu không có tài sản nào");

            var userId = await GetUserIdAsync(username);

            // Cập nhật tài sản: chuyển UseDepartmentId và StorageLocationNo
            var assetNos = handover.Details.Select(d => d.AssetNo).ToList();
            var assets = await _context.Assets_Information
                .Where(a => assetNos.Contains(a.AssetNo))
                .ToListAsync();

            foreach (var asset in assets)
            {
                asset.UseDepartmentId = handover.ToDepartmentId;
                if (!string.IsNullOrEmpty(handover.ToStorageLocationNo))
                    asset.StorageLocationNo = handover.ToStorageLocationNo;
                asset.UpdatedAt = DateTime.Now;
                asset.UpdatedBy = userId;
            }

            handover.Status = HandoverStatus.Confirmed;
            handover.ConfirmedAt = DateTime.Now;
            handover.ConfirmedBy = userId;
            handover.UpdatedAt = DateTime.Now;
            handover.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── HỦY (gộp Reject + Cancel) ────────────────────────────

        public async Task<HandoverOrderDto> CancelAsync(string id, string username)
        {
            var handover = await GetHandoverOrThrowAsync(id);

            if (handover.Status == HandoverStatus.Confirmed)
                throw new Exception("Không thể hủy phiếu đã hoàn thành");

            if (handover.Status == HandoverStatus.Cancelled)
                throw new Exception("Phiếu đã bị hủy trước đó");

            var userId = await GetUserIdAsync(username);

            handover.Status = HandoverStatus.Cancelled;
            handover.UpdatedAt = DateTime.Now;
            handover.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── THÊM TÀI SẢN VÀO PHIẾU ─────────────────────────────

        public async Task<HandoverOrderDto> AddAssetsAsync(string id, AddHandoverAssetsDto dto, string username)
        {
            var handover = await _context.Asset_Handovers
                .Include(h => h.Details)
                .FirstOrDefaultAsync(h => h.Id == id)
                ?? throw new Exception("Phiếu bàn giao không tồn tại");

            if (handover.Status != HandoverStatus.Draft && handover.Status != HandoverStatus.Pending)
                throw new Exception("Chỉ có thể thêm tài sản khi phiếu ở trạng thái Nháp hoặc Chờ duyệt");

            if (!dto.AssetNos.Any())
                throw new Exception("Danh sách tài sản không được rỗng");

            var userId = await GetUserIdAsync(username);

            // Lọc tài sản chưa có trong phiếu
            var existingAssetNos = handover.Details.Select(d => d.AssetNo).ToHashSet();
            var newAssetNos = dto.AssetNos.Where(n => !existingAssetNos.Contains(n)).ToList();

            if (!newAssetNos.Any())
                throw new Exception("Tất cả tài sản đã có trong phiếu");

            var assets = await _context.Assets_Information
                .AsNoTracking()
                .Where(a => newAssetNos.Contains(a.AssetNo) && !a.Is_Deleted)
                .ToListAsync();

            var notOwned = assets.Where(a => a.UseDepartmentId != handover.FromDepartmentId).ToList();
            if (notOwned.Any())
                throw new Exception($"Tài sản không thuộc đơn vị giao: {string.Join(", ", notOwned.Select(a => a.AssetNo))}");

            foreach (var asset in assets)
            {
                _context.Asset_Handover_Details.Add(new Asset_Handover_Details
                {
                    HandoverId = id,
                    AssetNo = asset.AssetNo,
                    PreviousUseDepartmentId = asset.UseDepartmentId,
                    PreviousStorageLocationNo = asset.StorageLocationNo
                });
            }

            handover.UpdatedAt = DateTime.Now;
            handover.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }

        // ─── XÓA TÀI SẢN KHỎI PHIẾU ─────────────────────────────

        public async Task RemoveAssetAsync(string id, int detailId, string username)
        {
            var handover = await GetHandoverOrThrowAsync(id);

            if (handover.Status != HandoverStatus.Draft && handover.Status != HandoverStatus.Pending)
                throw new Exception("Chỉ có thể xóa tài sản khi phiếu ở trạng thái Nháp hoặc Chờ duyệt");

            var detail = await _context.Asset_Handover_Details
                .FirstOrDefaultAsync(d => d.Id == detailId && d.HandoverId == id)
                ?? throw new Exception("Chi tiết không tồn tại trong phiếu");

            _context.Asset_Handover_Details.Remove(detail);
            await _context.SaveChangesAsync();
        }

        // ─── TÌM KIẾM TÀI SẢN CÓ THỂ BÀN GIAO ──────────────────

        public async Task<PagedResult<HandoverAssetCandidateDto>> SearchCandidateAssetsAsync(SearchHandoverAssetsDto dto)
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
                .Select(a => new HandoverAssetCandidateDto
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

            return new PagedResult<HandoverAssetCandidateDto> { Total = total, Items = items };
        }

        // ─── GET ───────────────────────────────────────────────────

        public async Task<HandoverOrderDto> GetAsync(string id)
        {
            var h = await _context.Asset_Handovers
                .AsNoTracking()
                .Include(h => h.FromDepartment)
                .Include(h => h.ToDepartment)
                .Include(h => h.Details)
                    .ThenInclude(d => d.Asset)
                        .ThenInclude(a => a.Model)
                            .ThenInclude(m => m.Small_Type)
                                .ThenInclude(s => s.Middle_Type)
                                    .ThenInclude(m => m.Big_Type)
                .FirstOrDefaultAsync(h => h.Id == id)
                ?? throw new Exception("Phiếu bàn giao không tồn tại");

            var createdBy = await GetUsernameByIdAsync(h.CreatedBy);
            var confirmedBy = h.ConfirmedBy.HasValue
                ? await GetUsernameByIdAsync(h.ConfirmedBy.Value)
                : null;

            return new HandoverOrderDto
            {
                Id = h.Id,
                FromDepartmentId = h.FromDepartmentId,
                FromDepartmentName = h.FromDepartment.Name,
                ToDepartmentId = h.ToDepartmentId,
                ToDepartmentName = h.ToDepartment.Name,
                FromStorageLocationNo = h.FromStorageLocationNo,
                ToStorageLocationNo = h.ToStorageLocationNo,
                Status = h.Status,
                Note = h.Note,
                CreatedAt = h.CreatedAt,
                CreatedByUserName = createdBy,
                ConfirmedAt = h.ConfirmedAt,
                ConfirmedByUserName = confirmedBy,
                Details = h.Details.Select(d => new HandoverDetailDto
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

        public async Task<PagedResult<HandoverOrderListDto>> SearchAsync(SearchHandoverOrdersDto dto)
        {
            var query = _context.Asset_Handovers
                .AsNoTracking()
                .Include(h => h.FromDepartment)
                .Include(h => h.ToDepartment)
                .Include(h => h.Details)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.FromDepartmentId))
                query = query.Where(h => h.FromDepartmentId == dto.FromDepartmentId);

            if (!string.IsNullOrEmpty(dto.ToDepartmentId))
                query = query.Where(h => h.ToDepartmentId == dto.ToDepartmentId);

            if (dto.Status.HasValue)
                query = query.Where(h => h.Status == dto.Status.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(h => h.CreatedAt)
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(h => new HandoverOrderListDto
                {
                    Id = h.Id,
                    FromDepartmentId = h.FromDepartmentId,
                    FromDepartmentName = h.FromDepartment.Name,
                    ToDepartmentId = h.ToDepartmentId,
                    ToDepartmentName = h.ToDepartment.Name,
                    Status = h.Status,
                    AssetCount = h.Details.Count,
                    CreatedAt = h.CreatedAt,
                    CreatedByUserName = _context.Users
                        .Where(u => u.Id == h.CreatedBy)
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return new PagedResult<HandoverOrderListDto> { Total = total, Items = items };
        }

        // ─── HELPERS ───────────────────────────────────────────────

        private async Task<Asset_Handovers> GetHandoverOrThrowAsync(string id) =>
            await _context.Asset_Handovers.FirstOrDefaultAsync(h => h.Id == id)
            ?? throw new Exception("Phiếu bàn giao không tồn tại");

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

        private async Task<string> GenerateHandoverIdAsync()
        {
            var prefix = "HO" + DateTime.Now.ToString("yyyyMM");
            var last = await _context.Asset_Handovers
                .AsNoTracking()
                .Where(h => h.Id.StartsWith(prefix))
                .OrderByDescending(h => h.Id)
                .Select(h => h.Id)
                .FirstOrDefaultAsync();

            int next = 1;
            if (last != null && int.TryParse(last[prefix.Length..], out int parsed))
                next = parsed + 1;

            return prefix + next.ToString("D4");
        }
        // ─── HOÀN TRẢ TÀI SẢN (Unlock) - chuyển về đơn vị gốc ────

        public async Task<HandoverOrderDto> UnlockAsync(string id, string username)
        {
            var handover = await _context.Asset_Handovers
                .Include(h => h.Details)
                .FirstOrDefaultAsync(h => h.Id == id)
                ?? throw new Exception("Phiếu bàn giao không tồn tại");

            if (handover.Status != HandoverStatus.Confirmed)
                throw new Exception("Chỉ có thể hoàn trả phiếu đã xác nhận");

            if (!handover.Details.Any())
                throw new Exception("Phiếu không có tài sản nào");

            var userId = await GetUserIdAsync(username);

            // Cập nhật tài sản: chuyển về UseDepartmentId cũ (FromDepartmentId) và StorageLocationNo cũ (FromStorageLocationNo)
            var assetNos = handover.Details.Select(d => d.AssetNo).ToList();
            var assets = await _context.Assets_Information
                .Where(a => assetNos.Contains(a.AssetNo))
                .ToListAsync();

            foreach (var detail in handover.Details)
            {
                var asset = assets.FirstOrDefault(a => a.AssetNo == detail.AssetNo);
                if (asset != null)
                {
                    asset.UseDepartmentId = handover.FromDepartmentId;
                    if (!string.IsNullOrEmpty(handover.FromStorageLocationNo))
                        asset.StorageLocationNo = handover.FromStorageLocationNo;
                    asset.UpdatedAt = DateTime.Now;
                    asset.UpdatedBy = userId;
                }
            }

            // Chuyển trạng thái phiếu thành Cancelled (để đánh dấu đã hoàn trả)
            handover.Status = HandoverStatus.Cancelled;
            handover.UpdatedAt = DateTime.Now;
            handover.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return await GetAsync(id);
        }
    }
}
