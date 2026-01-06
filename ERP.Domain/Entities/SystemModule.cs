using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class SystemModule
    {
        [Key]
        [StringLength(50)]
        public string Code { get; set; } = default!; public string Name { get; set; } = default!;

        public ICollection<UserSystem> UserSystems { get; set; } = new List<UserSystem>();
    }


}
