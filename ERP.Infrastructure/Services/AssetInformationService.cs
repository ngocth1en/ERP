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
        DepartmentId = g.Key.DepartmentId,
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

        // ================= INVENTORY ORDERS =================
        public async Task<InventoryOrderDto> CreateInventoryOrderAsync(CreateInventoryOrderDto dto, string? username)
        {
            // Validate model exists
            var model = await _context.Asset_Models
                .AsNoTracking()
                .Include(m => m.Middle_Type)
                .FirstOrDefaultAsync(m => m.Model_Id == dto.ModelId);

            if (model == null)
                throw new Exception($"ModelId [{dto.ModelId}] không tồn tại");

            var inventoryType = model.Middle_Type.Inventory_Check_Type ?? "1";

            // Get assets in DB
            var assetsInDb = await _context.Assets_Information
                .AsNoTracking()
                .Where(a => a.Model_Id == dto.ModelId && !a.Is_Deleted
                            && (string.IsNullOrEmpty(dto.DepartmentId) || a.UseDepartmentId == dto.DepartmentId)
                            && (string.IsNullOrEmpty(dto.CheckLocationNo) || a.StorageLocationNo == dto.CheckLocationNo))
                .Select(a => new { a.AssetNo, a.QRCode })
                .ToListAsync();

            var userId = await GetUserId(username ?? "");

            // Generate Inventory Order ID
            string orderId = await GenerateInventoryOrderId();

            var inventory = new Asset_Inventories
            {
                Id = orderId,
                Model_Id = dto.ModelId,
                DepartmentId = dto.DepartmentId,
                CheckLocationNo = dto.CheckLocationNo,
                Inventory_Type = inventoryType,
                ExpectedCount = assetsInDb.Count,
                Counted = 0,
                MissingCount = assetsInDb.Count,
                UnexpectedCount = 0,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                Note = dto.Note
            };

            _context.Asset_Inventories.Add(inventory);
            await _context.SaveChangesAsync();
            // ✅ Automatically add assets to details
            foreach (var asset in assetsInDb)
            {
                var detail = new Asset_Inventory_Details
                {
                    Id = asset.AssetNo+orderId,
                    InventoryId = orderId,
                    AssetNo = asset.AssetNo,
                    QRCode = asset.QRCode,
                    Status = "PENDING", // Chưa kiểm kê
                    Note = null
                };
                _context.Asset_Inventory_Details.Add(detail);
            }
            await _context.SaveChangesAsync();   // <-- Thiếu dòng này

            return await GetInventoryOrderAsync(inventory.Id);
        }

        public async Task<InventoryOrderDto> UpdateInventoryOrderAsync(string id, UpdateInventoryOrderDto dto, string? username)
        {
            var inventory = await _context.Asset_Inventories
                .Include(i => i.Details)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
                throw new Exception("Phiếu kiểm kê không tồn tại");

            var userId = await GetUserId(username ?? "");

            inventory.DepartmentId = dto.DepartmentId ?? inventory.DepartmentId;
            inventory.Note = dto.Note ?? inventory.Note;

            // Update details if provided
            if (dto.Details != null && dto.Details.Any())
            {
                // Remove existing details
                _context.Asset_Inventory_Details.RemoveRange(inventory.Details);

                // Add new details
                foreach (var detail in dto.Details)
                {
                    var detailEntity = new Asset_Inventory_Details
                    {
                        Id = Guid.NewGuid().ToString("N").Substring(0, 50), // Generate short ID for detail
                        InventoryId = id,
                        AssetNo = detail.AssetNo,
                        QRCode = detail.QRCode,
                        Status = detail.Status,
                        Note = detail.Note
                    };
                    _context.Asset_Inventory_Details.Add(detailEntity);
                }

                // Recalculate counts
                var found = dto.Details.Count(d => d.Status == "FOUND");
                var missing = dto.Details.Count(d => d.Status == "MISSING");
                var unexpected = dto.Details.Count(d => d.Status == "UNEXPECTED");

                inventory.Counted = found;
                inventory.MissingCount = missing;
                inventory.UnexpectedCount = unexpected;
            }

            await _context.SaveChangesAsync();

            return await GetInventoryOrderAsync(id);
        }

        public async Task<PagedResult<InventoryOrderListDto>> SearchInventoryOrdersAsync(int? modelId, string? departmentId, int? pageIndex, int? pageSize)
        {
            var page = pageIndex ?? 1;
            var size = pageSize ?? 10;

            var query = _context.Asset_Inventories
                .AsNoTracking()
                .AsQueryable();

            if (modelId.HasValue)
                query = query.Where(i => i.Model_Id == modelId.Value);

            if (!string.IsNullOrEmpty(departmentId))
                query = query.Where(i => i.DepartmentId == departmentId);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(i => new InventoryOrderListDto
                {
                    Id = i.Id,
                    ModelId = i.Model_Id,
                    DepartmentId = i.DepartmentId,
                    CheckLocationNo = i.CheckLocationNo,
                    InventoryType = i.Inventory_Type,
                    ExpectedCount = i.ExpectedCount,
                    Counted = i.Counted,
                    CreatedAt = i.CreatedAt,
                    CreatedByUserName = _context.Users
                        .Where(u => u.Id == i.CreatedBy)
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return new PagedResult<InventoryOrderListDto>
            {
                Total = total,
                Items = items
            };
        }

        public async Task<InventoryOrderDto> GetInventoryOrderAsync(string id)
        {
            var inventory = await _context.Asset_Inventories
                .AsNoTracking()
                .Include(i => i.Details)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
                throw new Exception("Phiếu kiểm kê không tồn tại");

            return new InventoryOrderDto
            {
                Id = inventory.Id,
                ModelId = inventory.Model_Id,
                DepartmentId = inventory.DepartmentId,
                CheckLocationNo = inventory.CheckLocationNo,
                InventoryType = inventory.Inventory_Type,
                ExpectedCount = inventory.ExpectedCount,
                Counted = inventory.Counted,
                MissingCount = inventory.MissingCount,
                UnexpectedCount = inventory.UnexpectedCount,
                CreatedAt = inventory.CreatedAt,
                Note = inventory.Note,
                Details = inventory.Details
                    .Select(d => new InventoryOrderDetailDto
                    {
                        Id = d.Id,
                        AssetNo = d.AssetNo ?? "",
                        QRCode = d.QRCode,
                        Status = d.Status,
                        Note = d.Note
                    })
                    .ToList()
            };
        }

        // ================= INVENTORY CHECK =================
        //public async Task<InventoryCheckResultDto> InventoryCheckAsync(InventoryCheckRequestDto dto, string username)
        //{
        //    // get model and its middle type
        //    var model = await _context.Asset_Models
        //        .AsNoTracking()
        //        .Include(m => m.Middle_Type)
        //        .FirstOrDefaultAsync(m => m.Model_Id == dto.ModelId);

        //    if (model == null)
        //        throw new Exception($"ModelId [{dto.ModelId}] không tồn tại");

        //    var inventoryType = model.Middle_Type.Inventory_Check_Type ?? "1";

        //    // get assets in DB
        //    var assetsInDb = await _context.Assets_Information
        //        .AsNoTracking()
        //        .Where(a => a.Model_Id == dto.ModelId && !a.Is_Deleted
        //                    && (string.IsNullOrEmpty(dto.DepartmentId) || a.UseDepartmentId == dto.DepartmentId)
        //                    && a.QRCode==dto.ScannedQRCodes)
        //        .Select(a => new { a.AssetNo, a.QRCode })
        //        .ToListAsync();

        //    var dbQRCodes = new HashSet<string>(assetsInDb
        //        .Select(a => (a.QRCode ?? a.AssetNo ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);

        //    var dbAssetNos = new HashSet<string>(assetsInDb
        //        .Select(a => (a.AssetNo ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);

        //    var result = new InventoryCheckResultDto
        //    {
        //        InventoryType = inventoryType,
        //        ExpectedCount = assetsInDb.Count
        //    };

        //    // prepare scanned list from DTO (support Items or ScannedQRCodes)
        //    List<InventoryCheckItemDto> scannedItems = new();
        //    if (dto.Items != null && dto.Items.Any())
        //    {
        //        scannedItems = dto.Items;
        //    }
        //    else if (dto.ScannedQRCodes != null && dto.ScannedQRCodes.Any())
        //    {
        //        scannedItems = dto.ScannedQRCodes
        //            .Where(s => !string.IsNullOrWhiteSpace(s))
        //            .Select(s => new InventoryCheckItemDto { Code = s.Trim() })
        //            .ToList();
        //    }

        //    var userId = await GetUserId(username);

        //    // Generate Inventory Order ID for this check
        //    string orderId = await GenerateInventoryOrderId();

        //    // Build inventory record to persist
        //    var inventoryRecord = new Asset_Inventories
        //    {
        //        Id = orderId,
        //        Model_Id = dto.ModelId,
        //        DepartmentId = dto.DepartmentId,
        //        Inventory_Type = inventoryType,
        //        ExpectedCount = assetsInDb.Count,
        //        CreatedAt = DateTime.Now,
        //        CreatedBy = userId,
        //        Note = null
        //    };

        //    if (inventoryType == "1")
        //    {
        //        var scanned = scannedItems
        //            .Where(i => !string.IsNullOrWhiteSpace(i.Code))
        //            .Select(i => i.Code.Trim())
        //            .Distinct(StringComparer.OrdinalIgnoreCase)
        //            .ToList();

        //        var scannedSet = new HashSet<string>(scanned, StringComparer.OrdinalIgnoreCase);

        //        var found = scanned.Where(s => dbQRCodes.Contains(s) || dbAssetNos.Contains(s)).ToList();
        //        var unexpected = scanned.Where(s => !(dbQRCodes.Contains(s) || dbAssetNos.Contains(s))).ToList();
        //        var missing = assetsInDb
        //            .Select(a => a.AssetNo ?? a.QRCode ?? string.Empty)
        //            .Where(x => !scannedSet.Contains(x))
        //            .Distinct(StringComparer.OrdinalIgnoreCase)
        //            .ToList();

        //        result.Counted = found.Distinct(StringComparer.OrdinalIgnoreCase).Count();
        //        result.FoundAssetNos = found.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        //        result.MissingAssetNos = missing;
        //        result.MissingCount = missing.Count;
        //        result.UnexpectedScannedQRCodes = unexpected;
        //        result.UnexpectedCount = unexpected.Count;

        //        inventoryRecord.Counted = result.Counted;
        //        inventoryRecord.MissingCount = result.MissingCount;
        //        inventoryRecord.UnexpectedCount = result.UnexpectedCount;

        //        // persist inventory record & details
        //        _context.Asset_Inventories.Add(inventoryRecord);

        //        // create details for found and unexpected with notes if provided
        //        // map scanned items to notes
        //        var noteMap = scannedItems
        //            .Where(i => !string.IsNullOrWhiteSpace(i.Code) && !string.IsNullOrWhiteSpace(i.Note))
        //            .ToDictionary(i => i.Code.Trim(), i => i.Note!, StringComparer.OrdinalIgnoreCase);

        //        foreach (var code in found.Distinct(StringComparer.OrdinalIgnoreCase))
        //        {
        //            var assetNo = dbAssetNos.Contains(code) ? code : (dbQRCodes.Contains(code) ? assetsInDb.FirstOrDefault(a => (a.QRCode ?? a.AssetNo) == code)?.AssetNo : code);
        //            var detail = new Asset_Inventory_Details
        //            {
        //                Id = Guid.NewGuid().ToString("N").Substring(0, 50),
        //                InventoryId = orderId,
        //                AssetNo = assetNo ?? code,
        //                QRCode = code,
        //                Status = "FOUND",
        //                Note = noteMap.ContainsKey(code) ? noteMap[code] : null
        //            };
        //            _context.Asset_Inventory_Details.Add(detail);
        //        }

        //        foreach (var code in unexpected.Distinct(StringComparer.OrdinalIgnoreCase))
        //        {
        //            var note = noteMap.ContainsKey(code) ? noteMap[code] : null;
        //            var detail = new Asset_Inventory_Details
        //            {
        //                Id = Guid.NewGuid().ToString("N").Substring(0, 50),
        //                InventoryId = orderId,
        //                AssetNo = code,
        //                QRCode = code,
        //                Status = "UNEXPECTED",
        //                Note = note
        //            };
        //            _context.Asset_Inventory_Details.Add(detail);
        //        }

        //        // create details for missing assets
        //        foreach (var missingAssetNo in missing.Distinct(StringComparer.OrdinalIgnoreCase))
        //        {
        //            var detail = new Asset_Inventory_Details
        //            {
        //                Id = Guid.NewGuid().ToString("N").Substring(0, 50),
        //                InventoryId = orderId,
        //                AssetNo = missingAssetNo,
        //                QRCode = null,
        //                Status = "MISSING",
        //                Note = null
        //            };
        //            _context.Asset_Inventory_Details.Add(detail);
        //        }

        //        await _context.SaveChangesAsync();
        //    }
        //    else
        //    {
        //        // Count mode
        //        var counted = dto.CountedQuantity ?? 0;
        //        result.Counted = counted;
        //        result.MissingCount = Math.Max(0, result.ExpectedCount - counted);
        //        result.UnexpectedCount = Math.Max(0, counted - result.ExpectedCount);

        //        inventoryRecord.Counted = counted;
        //        inventoryRecord.MissingCount = result.MissingCount;
        //        inventoryRecord.UnexpectedCount = result.UnexpectedCount;

        //        _context.Asset_Inventories.Add(inventoryRecord);
        //        await _context.SaveChangesAsync();
        //    }

        //    return result;
        //}
        // ================= INVENTORY CHECK =================
        public async Task<InventoryCheckResultDto> InventoryCheckAsync(InventoryCheckRequestDto dto, string username)
        {
            // Lấy đơn kiểm kê đã tạo
            var inventory = await _context.Asset_Inventories
                .Include(i => i.Details)
                .FirstOrDefaultAsync(i => i.Id == dto.InventoryId);

            if (inventory == null)
                throw new Exception($"Phiếu kiểm kê [{dto.InventoryId}] không tồn tại");

            var inventoryType = inventory.Inventory_Type ?? "1";

            var result = new InventoryCheckResultDto
            {
                InventoryType = inventoryType,
                ExpectedCount = inventory.ExpectedCount
            };

            if (inventoryType == "2")
            {
                // ===== SCAN MODE: cập nhật từng mã QR =====
                var code = (dto.ScannedQRCode ?? "").Trim();
                if (string.IsNullOrEmpty(code))
                    throw new Exception("Mã QR không được để trống");

                // Tìm detail khớp QRCode hoặc AssetNo
                var detail = inventory.Details
                    .FirstOrDefault(d =>
                        string.Equals(d.QRCode, code, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(d.AssetNo, code, StringComparison.OrdinalIgnoreCase));

                if (detail != null)
                {
                    // Tài sản có trong đơn → đánh dấu FOUND
                    detail.Status = "FOUND";
                }
                else
                {
                    // Tài sản không có trong đơn → UNEXPECTED
                    // ✅ Tìm AssetNo từ QR code trong DB
                    var assetFromDb = await _context.Assets_Information
                        .AsNoTracking()
                        .Where(a => !a.Is_Deleted &&
                                    (a.QRCode == code || a.AssetNo == code))
                        .Select(a => new { a.AssetNo })
                        .FirstOrDefaultAsync();

                    // Nếu không tìm thấy trong DB → dùng code làm AssetNo
                    var resolvedAssetNo = assetFromDb?.AssetNo ?? code;

                    // ✅ Id = AssetNo + InventoryId (giống pattern tạo đơn)
                    var unexpectedId = resolvedAssetNo + dto.InventoryId;

                    _context.Asset_Inventory_Details.Add(new Asset_Inventory_Details
                    {
                        Id = unexpectedId,
                        InventoryId = dto.InventoryId,
                        AssetNo = resolvedAssetNo,
                        QRCode = code,
                        Status = "UNEXPECTED",
                        Note = null
                    });

                    await _context.SaveChangesAsync();
                    await _context.Entry(inventory).ReloadAsync();
                    await _context.Entry(inventory).Collection(i => i.Details).LoadAsync();
                }

                // Tính lại counts từ details
                var allDetails = inventory.Details.ToList();
                inventory.Counted = allDetails.Count(d => d.Status == "FOUND");
                inventory.MissingCount = allDetails.Count(d => d.Status == "PENDING");
                inventory.UnexpectedCount = allDetails.Count(d => d.Status == "UNEXPECTED");

                result.Counted = inventory.Counted;
                result.MissingCount = inventory.MissingCount;
                result.UnexpectedCount = inventory.UnexpectedCount;
            }
            else
            {
                // ===== COUNT MODE: nhập số lượng =====
                var counted = dto.CountedQuantity ?? 0;
                inventory.Counted = counted;
                inventory.MissingCount = Math.Max(0, inventory.ExpectedCount - counted);
                inventory.UnexpectedCount = Math.Max(0, counted - inventory.ExpectedCount);

                result.Counted = inventory.Counted;
                result.MissingCount = inventory.MissingCount;
                result.UnexpectedCount = inventory.UnexpectedCount;
            }

            await _context.SaveChangesAsync();

            return result;
        }
        // ================= GENERATE INVENTORY ORDER ID =================
        private async Task<string> GenerateInventoryOrderId()
        {
            string datePrefix = DateTime.Now.ToString("yyyyMMdd"); // YYYYMMdd

            // Get all inventory orders created today
            var lastOrder = await _context.Asset_Inventories
                .AsNoTracking()
                .Where(x => x.CreatedAt.Year == DateTime.Now.Year
                            && x.CreatedAt.Month == DateTime.Now.Month
                            && x.CreatedAt.Day == DateTime.Now.Day)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Id.ToString())
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (!string.IsNullOrEmpty(lastOrder))
            {
                // Extract the last 4 digits (sequence number)
                string numberPart = lastOrder.Substring(lastOrder.Length - 4); // Last 4 chars
                if (int.TryParse(numberPart, out int parsedNumber))
                {
                    nextNumber = parsedNumber + 1;
                }
            }

            return datePrefix + nextNumber.ToString("D4"); // YYYYMMddxxxx
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
            return assetNo;
        }

        private async Task<Guid> GetUserId(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return Guid.Empty;

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null)
                throw new Exception($"User [{userName}] không tồn tại");

            return user.Id;
        }
    }
}