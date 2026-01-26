
using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Application.Common;
using ERP.Domain.Entities;
using ERP.Infrastructure.ERP.Domain.Entities;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Asset.Information.Services
{
    public class AssetInformationService : IAssetInformationService
    {
        private readonly AppDbContext _context;

        public AssetInformationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<AssetStatisticResultDto>> SearchStatisticAsync(
    AssetStatisticSearchDto dto)
        {
            var query = _context.Assets_Information
                .AsNoTracking()
                .Include(x => x.Model)
                    .ThenInclude(s => s.Small_Type)
                        .ThenInclude(m => m.Middle_Type)
                            .ThenInclude(b => b.Big_Type)
                .AsQueryable();

            #region FILTER

            if (!string.IsNullOrEmpty(dto.DepartmentId))
                query = query.Where(x => x.DepartmentId == dto.DepartmentId);

            if (!string.IsNullOrEmpty(dto.AssetNo))
                query = query.Where(x => x.AssetNo.Contains(dto.AssetNo));

            if (dto.Status.HasValue)
                query = query.Where(x => x.Status == dto.Status.Value);

            if (dto.MiddleTypeId.HasValue)
                query = query.Where(x =>
                    x.Model.Middle_Type_Id == dto.MiddleTypeId.Value);

            if (dto.BigTypeId.HasValue)
                query = query.Where(x =>
                    x.Model.Big_Type_Id == dto.BigTypeId.Value);

            #endregion

            var groupedQuery = query
    .GroupBy(x => new
    {
        x.Model_Id,
        x.DepartmentId,
        BigName = x.Model.Big_Type.Big_Type_Name_VN,
        MiddleName = x.Model.Middle_Type.Middle_Type_Name_VN,
        SmallName = x.Model.Small_Type.Small_Type_Name_VN,
        ModelName = x.Model.Model_Name
    })
    .Select(g => new AssetStatisticResultDto
    {
        ModelId = g.Key.Model_Id,   
        DepartmentId=g.Key.DepartmentId,
        AssetName = g.Key.BigName + " - " +
                    g.Key.MiddleName + " - " +
                    g.Key.SmallName,

        ModelName = g.Key.ModelName,
        TotalAssets = g.Count()
    });


            // tổng số group
            var total = await groupedQuery.CountAsync();

            var items = await groupedQuery
                .OrderBy(x => x.AssetName)
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();

            return new PagedResult<AssetStatisticResultDto>
            {
                Total = total,
                Items = items
            };
        }
        public async Task<PagedResult<AssetByModelResultDto>> SearchAssetsByModelAsync(
    AssetByModelSearchDto dto)
        {
            var query = _context.Assets_Information
                .AsNoTracking()
                .Include(x => x.Model)
                    .ThenInclude(s => s.Small_Type)
                        .ThenInclude(m => m.Middle_Type)
                            .ThenInclude(b => b.Big_Type)
                .Where(x => x.Model_Id == dto.ModelId);

            #region FILTER

            if (!string.IsNullOrEmpty(dto.AssetNo))
                query = query.Where(x => x.AssetNo.Contains(dto.AssetNo));

            if (!string.IsNullOrEmpty(dto.DepartmentId))
                query = query.Where(x => x.DepartmentId == dto.DepartmentId);

            if (dto.Status.HasValue)
                query = query.Where(x => x.Status == dto.Status.Value);

            #endregion

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.AssetNo)
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(x => new AssetByModelResultDto
                {
                    AssetNo = x.AssetNo,

                    AssetName =
                        x.Model.Big_Type.Big_Type_Name_VN + " - " +
                        x.Model.Middle_Type.Middle_Type_Name_VN + " - " +
                        x.Model.Small_Type.Small_Type_Name_VN,

                    ModelName = x.Model.Model_Name,
                    DepartmentId = x.DepartmentId,
                    Status = x.Status
                })
                .ToListAsync();

            return new PagedResult<AssetByModelResultDto>
            {
                Total = total,
                Items = items
            };
        }
        public async Task CreateAsync(CreateAssetDto dto, string username)
        {
            string assetNo = await GenerateAssetNo();
            string qrCode = GenerateQRCode(assetNo);
            var userId = await GetUserId(username);
            var entity = new Assets_Information
            {
                AssetNo = assetNo,
                QRCode = qrCode,
                DepartmentId = dto.DepartmentId,
                UseDepartmentId = dto.UseDepartmentId,
                Model_Id = dto.Model_Id,
                Status = dto.Status,
                StorageLocationNo = dto.StorageLocationNo,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                Is_Deleted = false
            };

            _context.Assets_Information.Add(entity);
            await _context.SaveChangesAsync();
        }

        // ================= UPDATE =================
        public async Task UpdateAsync(string assetNo, UpdateAssetDto dto, string username)
        {
            var entity = await _context.Assets_Information
                .FirstOrDefaultAsync(x => x.AssetNo == assetNo && !x.Is_Deleted);
            var userId = await GetUserId(username);
            if (entity == null)
                throw new Exception("Asset không tồn tại");

            entity.DepartmentId = dto.DepartmentId;
            entity.UseDepartmentId = dto.UseDepartmentId;
            entity.Model_Id = dto.Model_Id;
            entity.Status = dto.Status;
            entity.StorageLocationNo = dto.StorageLocationNo;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync();
        }

        // ================= GENERATE ASSETNO =================
        private async Task<string> GenerateAssetNo()
        {
            string prefix = "AS" + DateTime.Now.ToString("yyyyMM");

            var lastAsset = await _context.Assets_Information
                .Where(x => x.AssetNo.StartsWith(prefix))
                .OrderByDescending(x => x.AssetNo)
                .Select(x => x.AssetNo)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastAsset != null)
            {
                string numberPart = lastAsset.Substring(prefix.Length); // 0001
                int.TryParse(numberPart, out nextNumber);
                nextNumber++;
            }

            return prefix + nextNumber.ToString("D4");
        }

        // ================= GENERATE QRCODE =================
        private string GenerateQRCode(string assetNo)
        {
            // Có thể chỉ dùng AssetNo
            return assetNo;

            // Hoặc:
            // return $"{assetNo}|{Guid.NewGuid()}";
        }
        private async Task<Guid> GetUserId(string userName)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null)
                throw new Exception($"User [{userName}] không tồn tại");

            return user.Id;
        }
    }
}
