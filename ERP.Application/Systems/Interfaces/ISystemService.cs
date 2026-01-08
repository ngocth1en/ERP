using ERP.Application.Systems.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Interfaces
{
    public interface ISystemService
    {
        Task<List<SystemOptionDto>> GetSystemsByUserAsync(string  username);
        Task<List<SystemOptionDto>> GetAllAsync();
        Task<List<SystemModuleDto>> GetAllInfoSystemAsync();
        Task UpdateAsync(string code, UpdateSystemModuleDto dto);
        Task<List<SystemGuideFileDto>> GetGuideFilesAsync(string systemCode);
        Task UploadGuideFileAsync(UploadSystemGuideFileDto dto);
    }
}
