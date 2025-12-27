using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enum.ASM
{
    public enum AssetStatus
    {
        NotUsed = 1,        // Không sử dụng
        Waiting = 2,        // Chờ sử dụng
        InUse = 3,          // Đang sử dụng
        Scrap = 4,          // Phế
        Liquidated = 5      // Thanh lý
    }
}
