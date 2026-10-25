using ERP.Application.Common;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Services
{
    /// <summary>
    /// Triển khai sinh mã tuần tự dựa trên bảng Id_Sequences,
    /// dùng câu lệnh MERGE + OUTPUT để đảm bảo tăng số nguyên tử (atomic),
    /// tránh race condition khi nhiều request tạo bản ghi cùng thời điểm.
    /// </summary>
    public class SequenceGenerator : ISequenceGenerator
    {
        private readonly AppDbContext _context;

        public SequenceGenerator(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> NextAsync(string prefix, int digitCount = 4, CancellationToken ct = default)
        {
            // MERGE ... OUTPUT đảm bảo tăng LastValue nguyên tử ở mức row-lock,
            // không cần giữ transaction mức cao (Serializable) nên hạn chế deadlock khi tải lớn.
            const string sql = @"
                MERGE dbo.Id_Sequences WITH (HOLDLOCK) AS target
                USING (SELECT @Prefix AS Prefix) AS source
                ON target.Prefix = source.Prefix
                WHEN MATCHED THEN
                    UPDATE SET LastValue = target.LastValue + 1
                WHEN NOT MATCHED THEN
                    INSERT (Prefix, LastValue) VALUES (source.Prefix, 1)
                OUTPUT inserted.LastValue;";

            var prefixParam = new SqlParameter("@Prefix", prefix);

            var result = await _context.Database
                .SqlQueryRaw<int>(sql, prefixParam)
                .ToListAsync(ct);

            var nextValue = result.Count > 0 ? result[0] : 1;

            return prefix + nextValue.ToString(new string('0', digitCount));
        }
    }
}
