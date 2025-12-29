using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Dtos
{
    public class TranslationUpsertDto
    {
        public string Key { get; set; }
        public string LanguageCode { get; set; }
        public string Value { get; set; }
    }

}
