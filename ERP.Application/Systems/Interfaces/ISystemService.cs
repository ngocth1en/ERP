using ERP.Application.Systems.Dtos.Menu;
using ERP.Application.Systems.Dtos.Permission;
using ERP.Application.Systems.Dtos.System;
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
        Task<List<SystemOptionDto>> GetSystemsByUserNotAllowedAsync(string username);
        Task<List<SystemOptionDto>> GetAllAsync();
        Task<List<SystemModuleDto>> GetAllInfoSystemAsync();
        //Task UpdateAsync(string code, UpdateSystemModuleDto dto);
        Task<SystemModuleDto> UpdateAsync(string code, UpdateSystemModuleDto dto);
        Task GrantAsync(GrantUserSystemDto dto);
        Task RevokeAsync(GrantUserSystemDto dto);
        Task<List<SystemGuideFileDto>> GetGuideFilesAsync(string systemCode);
        Task UploadGuideFileAsync(UploadSystemGuideFileDto dto);

    }
}
