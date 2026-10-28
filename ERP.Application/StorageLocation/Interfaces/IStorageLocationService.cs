using ERP.Application.Common;
using ERP.Application.StorageLocation.Dtos.StorageLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.StorageLocation.Interfaces
{
    public interface IStorageLocationService
    {
        Task<List<StorageLocationDto>> GetAllAsync(string languageCode);
        Task<StorageLocationDto?> GetByIdAsync(string storageLocationNo, string languageCode);
        Task UpdateStatusAsync(string storageLocationNo, int status);
        Task<PagedResult<StorageLocationListDto>> SearchAsync(SearchStorageLocationDto dto);
        Task CreateAsync(CreateStorageLocationDto dto);
        Task UpdateAsync(string storageLocationNo, UpdateStorageLocationDto dto);

        /// <summary>
        /// Lấy sơ đồ kho 3D theo tầng: danh sách vị trí + số lượng tài sản mỗi vị trí,
        /// dùng để render floor plan 3D (CAD/3D Visualization).
        /// </summary>
        Task<WarehouseFloorPlanDto> GetFloorPlanAsync(int floor);

        /// <summary>
        /// Lấy danh sách tài sản tại 1 vị trí lưu trữ cụ thể (dùng khi raycasting click chọn ô trong sơ đồ 3D).
        /// </summary>
        Task<List<WarehouseCellAssetDto>> GetAssetsByLocationAsync(string storageLocationNo);
    }

}