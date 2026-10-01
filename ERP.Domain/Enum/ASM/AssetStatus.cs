using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enum.ASM
{
    public enum AssetStatus
    {
        NotUsed = 0,        // Không sử dụng
        Waiting = 1,        // Chờ sử dụng
        InUse = 2,          // Đang sử dụng
        Scrap = 3,          // Phế
        Liquidated = 4      // Thanh lý
    }
}
