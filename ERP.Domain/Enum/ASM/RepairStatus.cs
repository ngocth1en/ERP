using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enum.ASM
{
    public enum RepairStatus
    {
        Draft = 1,       // Nháp
        Pending = 2,     // Chờ duyệt
        InProgress = 3,  // Đang sửa chữa
        Completed = 4,   // Hoàn thành (tài sản phục hồi sử dụng)
        Cancelled = 5    // Hủy
    }
}
