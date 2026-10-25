using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enum.ASM
{
    public enum LiquidationStatus
    {
        Draft = 1,      // Nháp
        Pending = 2,    // Chờ duyệt
        Confirmed = 3,  // Đã thanh lý (hoàn thành)
        Cancelled = 4   // Hủy
    }
}
