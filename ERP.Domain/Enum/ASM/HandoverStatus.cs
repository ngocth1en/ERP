using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enum.ASM
{
    public enum HandoverStatus
    {
        Draft = 1,      // Nháp
        Pending = 2,    // Chờ duyệt
        Confirmed = 3,  // Hoàn thành
        Rejected = 4,   // Từ chối
        Cancelled = 5   // Hủy
    }
}
