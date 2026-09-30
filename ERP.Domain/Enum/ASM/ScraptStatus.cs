using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enum.ASM
{
    public enum ScrapStatus
    {
        Draft = 1,      // Nháp
        Pending = 2,    // Chờ duyệt
        Confirmed = 3,  // Đã báo phế (hoàn thành)
        Rejected = 4,   // Từ chối
        Cancelled = 5   // Hủy
    }
}
