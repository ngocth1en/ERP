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
        public string Code { get; set; } = default!;

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = default!;

        // Trạng thái hệ thống
        public bool IsActive { get; set; } = true; // true: sử dụng, false: ngừng

        // Ngày cập nhật
        public DateTime? UpdatedAt { get; set; }

        public ICollection<UserSystem> UserSystems { get; set; } = new List<UserSystem>();
        public ICollection<SystemGuideFile> GuideFiles { get; set; }
    }


}
