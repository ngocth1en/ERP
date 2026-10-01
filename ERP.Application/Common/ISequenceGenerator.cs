using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Common
{
    /// <summary>
    /// Sinh mã tuần tự dạng "PREFIX####" một cách an toàn (thread-safe / process-safe)
    /// khi có nhiều request tạo bản ghi cùng lúc.
    /// </summary>
    public interface ISequenceGenerator
    {
        /// <summary>
        /// Cấp phát số tiếp theo cho 1 prefix (VD: "HO202601") và trả về mã đầy đủ
        /// dạng PREFIX + số được pad theo digitCount (mặc định 4 chữ số).
        /// </summary>
        Task<string> NextAsync(string prefix, int digitCount = 4, CancellationToken ct = default);
    }
}
