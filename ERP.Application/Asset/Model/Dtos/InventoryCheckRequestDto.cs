using System.Collections.Generic;

namespace ERP.Application.Asset.Model.Dtos
{
    //public class InventoryCheckRequestDto
    //{
    //    public int ModelId { get; set; }

    //    public string? DepartmentId { get; set; }

    //    // N?u client ch? g?i list m? v? ghi ch? cho t?ng m? (scan mode)
    //    public List<InventoryCheckItemDto>? Items { get; set; }

    //    // D?nh cho ki?u ??m: ng??i d?ng nh?p t?ng s?
    //    public int? CountedQuantity { get; set; }

    //    // Backward compatibility: n?u client v?n g?i ScannedQRCodes (string list)
    //    public string ScannedQRCodes { get; set; }
    //}
    public class InventoryCheckRequestDto
    {
        // ID đơn kiểm kê đã tạo sẵn
        public string InventoryId { get; set; } = default!;

        // Mã QR đơn lẻ được quét (1 lần quét = 1 mã)
        public string ScannedQRCode { get; set; } = default!;

        // Dành cho kiểu đếm số lượng (không dùng QR)
        public int? CountedQuantity { get; set; }
    }
}