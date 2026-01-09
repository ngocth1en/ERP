using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class SystemGuideFile
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string SystemCode { get; set; } = default!;
        public SystemModule SystemModule { get; set; } = default!;

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = default!;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = default!;

        public DateTime UploadedAt { get; set; }
    }

}
