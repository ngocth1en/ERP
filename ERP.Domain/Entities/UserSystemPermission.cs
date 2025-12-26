using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class UserSystemPermission
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string SystemCode { get; set; } = null!;
        public SystemModule SystemModule { get; set; } = null!;

        public string Permission { get; set; } = null!; // Admin / User / Viewer
    }


}
