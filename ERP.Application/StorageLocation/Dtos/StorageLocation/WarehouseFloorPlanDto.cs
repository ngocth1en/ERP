using System.Collections.Generic;

namespace ERP.Application.StorageLocation.Dtos.StorageLocation
{
    /// <summary>
    /// Thông tin 1 ô vị trí lưu trữ trong sơ đồ kho 3D, dùng cho CAD/3D Warehouse Floor Plan Viewer.
    /// </summary>
    public class WarehouseCellDto
    {
        public string StorageLocationNo { get; set; } = default!;
        public string NameVN { get; set; } = default!;
        public string DepartmentName { get; set; } = default!;

        public int Status { get; set; } // 1 = Đang dùng, 2 = Trống, 3 = Báo phế
        public int InUse { get; set; }

        /// <summary>Số lượng tài sản hiện có tại vị trí này.</summary>
        public int AssetCount { get; set; }

        /// <summary>Vị trí tương đối trong lưới sơ đồ (được backend tính toán theo thứ tự).</summary>
        public int GridX { get; set; }
        public int GridZ { get; set; }
    }

    /// <summary>
    /// Chi tiết tài sản tại 1 vị trí lưu trữ, dùng khi raycasting click vào ô trong sơ đồ 3D.
    /// </summary>
    public class WarehouseCellAssetDto
    {
        public string AssetNo { get; set; } = default!;
        public string ModelName { get; set; } = default!;
        public string AssetFullName { get; set; } = default!; // Big - Middle - Small
        public int Status { get; set; }
        public string? Model3DUrl { get; set; }
        public string? ImageDataBase64 { get; set; }
    }

    public class WarehouseFloorPlanDto
    {
        public int Floor { get; set; }
        public List<WarehouseCellDto> Cells { get; set; } = new();
    }
}