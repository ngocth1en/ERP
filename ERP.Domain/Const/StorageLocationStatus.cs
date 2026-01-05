using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Const
{
    public static class StorageLocationStatus
    {
        public const int InUse = 0;       // Đang sử dụng
        public const int Empty = 1;       // Trống
        public const int Repairing = 2;   // Đang sửa
        public const int Scrap = 3;       // Báo phế
    }

}
