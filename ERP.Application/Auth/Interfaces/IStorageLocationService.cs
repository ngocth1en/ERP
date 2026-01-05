using ERP.Application.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Interfaces
{
    public interface IStorageLocationService
    {
        Task<List<StorageLocationDto>> GetAllAsync(string languageCode);
        Task<StorageLocationDto?> GetByIdAsync(string storageLocationNo, string languageCode);
        Task UpdateStatusAsync(string storageLocationNo, int status);
    }

}
