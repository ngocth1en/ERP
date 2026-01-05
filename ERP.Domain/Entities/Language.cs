using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class Language
    {
        public string Code { get; set; } = default!; // vi, en, jp
        public string Name { get; set; } = default!;
        public ICollection<Translation> Translations { get; set; }
       = new List<Translation>();
    }

}
