using ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Interfaces.Repositories
{
    public interface IPermissionRepository
    {
        Task<List<SystemModule>> GetSystemsByUserAsync(Guid userId);

        Task<UserSystemPermission?> GetPermissionAsync(
            Guid userId,
            string systemCode);
    }
}
