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
    }

}
