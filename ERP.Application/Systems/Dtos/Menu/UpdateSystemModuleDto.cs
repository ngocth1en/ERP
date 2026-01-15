using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos.Menu
{
    public class UpdateSystemModuleDto
    {
        [Required]
        public string Name { get; set; } = default!;

        public bool IsActive { get; set; }
    }

}
