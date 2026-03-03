using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Application.Common;
using ERP.Domain.Entities;
using ERP.Infrastructure.ERP.Domain.Entities;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // INVENTORY CHECK (now persists inventory + details)
        public async Task<InventoryCheckResultDto> InventoryCheckAsync(InventoryCheckRequestDto dto, string username)
        {
            // get model and its middle type
            var model = await _context.Asset_Models
                .AsNoTracking()
                .Include(m => m.Middle_Type)
                .FirstOrDefaultAsync(m => m.Model_Id == dto.ModelId);

            if (model == null)
                throw new Exception($"ModelId [{dto.ModelId}] không tồn tại");

            var inventoryType = model.Middle_Type.Inventory_Check_Type ?? "1";

            // get assets in DB
            var assetsInDb = await _context.Assets_Information
                .AsNoTracking()
                .Where(a => a.Model_Id == dto.ModelId && !a.Is_Deleted
                            && (string.IsNullOrEmpty(dto.DepartmentId) || a.DepartmentId == dto.DepartmentId))
                .Select(a => new { a.AssetNo, a.QRCode })
                .ToListAsync();

            var dbQRCodes = new HashSet<string>(assetsInDb
                .Select(a => (a.QRCode ?? a.AssetNo ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);

            var dbAssetNos = new HashSet<string>(assetsInDb
                .Select(a => (a.AssetNo ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);

            var result = new InventoryCheckResultDto
            {
                InventoryType = inventoryType,
                ExpectedCount = assetsInDb.Count
            };

            // prepare scanned list from DTO (support Items or ScannedQRCodes)
            List<InventoryCheckItemDto> scannedItems = new();
            if (dto.Items != null && dto.Items.Any())
            {
                scannedItems = dto.Items;
            }
            else if (dto.ScannedQRCodes != null && dto.ScannedQRCodes.Any())
            {
                scannedItems = dto.ScannedQRCodes
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => new InventoryCheckItemDto { Code = s.Trim() })
                    .ToList();
            }

            var userId = await GetUserId(username);

            // Build inventory record to persist
            var inventoryRecord = new Asset_Inventories
            {
                Id = Guid.NewGuid(),
                Model_Id = dto.ModelId,
                DepartmentId = dto.DepartmentId,
                Inventory_Type = inventoryType,
                ExpectedCount = assetsInDb.Count,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                Note = null
            };

            if (inventoryType == "1")
            {
                var scanned = scannedItems
                    .Where(i => !string.IsNullOrWhiteSpace(i.Code))
                    .Select(i => i.Code.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var scannedSet = new HashSet<string>(scanned, StringComparer.OrdinalIgnoreCase);

                var found = scanned.Where(s => dbQRCodes.Contains(s) || dbAssetNos.Contains(s)).ToList();
                var unexpected = scanned.Where(s => !(dbQRCodes.Contains(s) || dbAssetNos.Contains(s))).ToList();
                var missing = assetsInDb
                    .Select(a => a.AssetNo ?? a.QRCode ?? string.Empty)
                    .Where(x => !scannedSet.Contains(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                result.Counted = found.Distinct(StringComparer.OrdinalIgnoreCase).Count();
                result.FoundAssetNos = found.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                result.MissingAssetNos = missing;
                result.MissingCount = missing.Count;
                result.UnexpectedScannedQRCodes = unexpected;
                result.UnexpectedCount = unexpected.Count;

                inventoryRecord.Counted = result.Counted;
                inventoryRecord.MissingCount = result.MissingCount;
                inventoryRecord.UnexpectedCount = result.UnexpectedCount;

                // persist inventory record & details
                _context.Asset_Inventories.Add(inventoryRecord);

                // create details for found and unexpected with notes if provided
                // map scanned items to notes
                var noteMap = scannedItems
                    .Where(i => !string.IsNullOrWhiteSpace(i.Code) && !string.IsNullOrWhiteSpace(i.Note))
                    .ToDictionary(i => i.Code.Trim(), i => i.Note!, StringComparer.OrdinalIgnoreCase);

                foreach (var code in found.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var assetNo = dbAssetNos.Contains(code) ? code : (dbQRCodes.Contains(code) ? assetsInDb.FirstOrDefault(a => (a.QRCode ?? a.AssetNo) == code)?.AssetNo : code);
                    var detail = new Asset_Inventory_Details
                    {
                        Id = Guid.NewGuid(),
                        InventoryId = inventoryRecord.Id,
                        AssetNo = assetNo ?? code,
                        QRCode = code,
                        Status = "FOUND",
                        Note = noteMap.ContainsKey(code) ? noteMap[code] : null
                    };
                    _context.Asset_Inventory_Details.Add(detail);
                }

                foreach (var code in unexpected.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var note = noteMap.ContainsKey(code) ? noteMap[code] : null;
                    var detail = new Asset_Inventory_Details
                    {
                        Id = Guid.NewGuid(),
                        InventoryId = inventoryRecord.Id,
                        AssetNo = code,
                        QRCode = code,
                        Status = "UNEXPECTED",
                        Note = note
                    };
                    _context.Asset_Inventory_Details.Add(detail);
                }

                // create details for missing assets
                foreach (var missingAssetNo in missing.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var detail = new Asset_Inventory_Details
                    {
                        Id = Guid.NewGuid(),
                        InventoryId = inventoryRecord.Id,
                        AssetNo = missingAssetNo,
                        QRCode = null,
                        Status = "MISSING",
                        Note = null
                    };
                    _context.Asset_Inventory_Details.Add(detail);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                // Count mode
                var counted = dto.CountedQuantity ?? 0;
                result.Counted = counted;
                result.MissingCount = Math.Max(0, result.ExpectedCount - counted);
                result.UnexpectedCount = Math.Max(0, counted - result.ExpectedCount);

                inventoryRecord.Counted = counted;
                inventoryRecord.MissingCount = result.MissingCount;
                inventoryRecord.UnexpectedCount = result.UnexpectedCount;

                _context.Asset_Inventories.Add(inventoryRecord);
                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<List<MonthlyInventoryDto>> GetMonthlyInventorySummaryAsync(int modelId, string? departmentId, int months = 6)
        {
            var since = DateTime.UtcNow.AddMonths(-months + 1);
            var query = _context.Asset_Inventories
                .AsNoTracking()
                .Where(i => i.Model_Id == modelId && i.CreatedAt >= since);

            if (!string.IsNullOrEmpty(departmentId))
                query = query.Where(i => i.DepartmentId == departmentId);

            var grouped = await query
                .GroupBy(i => new { Year = i.CreatedAt.Year, Month = i.CreatedAt.Month })
                .Select(g => new MonthlyInventoryDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    ExpectedCount = g.Sum(x => x.ExpectedCount),
                    Counted = g.Sum(x => x.Counted),
                    MissingCount = g.Sum(x => x.MissingCount),
                    UnexpectedCount = g.Sum(x => x.UnexpectedCount)
                })
                .ToListAsync();

            var result = new List<MonthlyInventoryDto>();
            var now = DateTime.UtcNow;
            for (int i = months - 1; i >= 0; i--)
            {
                var dt = now.AddMonths(-i);
                var item = grouped.FirstOrDefault(g => g.Year == dt.Year && g.Month == dt.Month);
                if (item == null)
                {
                    result.Add(new MonthlyInventoryDto
                    {
                        Year = dt.Year,
                        Month = dt.Month,
                        ExpectedCount = 0,
                        Counted = 0,
                        MissingCount = 0,
                        UnexpectedCount = 0
                    });
                }
                else
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}
