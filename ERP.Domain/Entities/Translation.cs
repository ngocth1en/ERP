using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class Translation
    {
        public Guid Id { get; set; }

        public string LanguageCode { get; set; } = default!;
        public string Key { get; set; } = default!;   // VD: ASSET_ADD
        public string Value { get; set; } = default!; // Giá trị hiển thị

        public Language Language { get; set; } = default!;
    }

}
