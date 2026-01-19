using ERP.Application.Department.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/departments")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentsController(IDepartmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách bộ phận (ComboBox)
        /// </summary>
        [HttpGet("options")]
        public async Task<IActionResult> GetOptions()
        {
            return Ok(await _service.GetOptionsAsync());
        }
    }
}
