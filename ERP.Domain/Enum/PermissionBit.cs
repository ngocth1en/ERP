using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Enum
{
    public enum PermissionBit
    {
        Query = 0,
        Insert = 1,
        Modify = 2,
        Delete = 3,
        Confirm = 4,
        Unlock = 5,
        Print = 6,
        ExportExcel = 7,
        Db1 = 8,
        Db2 = 9
    }
}
