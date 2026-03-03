using System.Collections.Generic;

namespace ERP.Application.Asset.Model.Dtos
{
    public class InventoryCheckRequestDto
    {
        public int ModelId { get; set; }

        public string? DepartmentId { get; set; }

        // N?u client ch? g?i list m? v? ghi ch? cho t?ng m? (scan mode)
        public List<InventoryCheckItemDto>? Items { get; set; }

        // D?nh cho ki?u ??m: ng??i d?ng nh?p t?ng s?
        public int? CountedQuantity { get; set; }

        // Backward compatibility: n?u client v?n g?i ScannedQRCodes (string list)
        public List<string>? ScannedQRCodes { get; set; }
    }
}