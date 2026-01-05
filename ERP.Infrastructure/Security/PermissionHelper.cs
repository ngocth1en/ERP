using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Security
{
    public static class PermissionHelper
    {
        public static bool HasPermission(string mask, int index)
        {
            if (string.IsNullOrEmpty(mask)) return false;
            if (mask.Length <= index) return false;
            return mask[index] == '1';
        }
    }

}
