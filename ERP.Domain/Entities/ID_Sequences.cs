using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    /// <summary>
    /// Bảng đếm dùng để sinh mã tuần tự (VD: HO202601####, SC202601####...)
    /// một cách an toàn khi có nhiều request đồng thời (tránh race condition).
    /// </summary>
    public class Id_Sequences
    {
        /// <summary>Tiền tố định danh, VD: "HO202601", "SC202601"</summary>
        public string Prefix { get; set; } = null!;

        /// <summary>Giá trị số cuối cùng đã cấp phát cho prefix này</summary>
        public int LastValue { get; set; }
    }
}
