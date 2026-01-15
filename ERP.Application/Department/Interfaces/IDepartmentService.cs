using ERP.Application.Department.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Department.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentOptionDto>> GetOptionsAsync();
    }
}
