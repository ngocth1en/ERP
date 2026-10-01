using ClosedXML.Excel;
using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Domain.Enum.ASM;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Services
{
    public class AssetReportService : IAssetReportService
    {
        private readonly AppDbContext _context;

        public AssetReportService(AppDbContext context)
        {
            _context = context;
        }

        private static readonly Dictionary<int, string> StatusLabels = new()
        {
            { (int)AssetStatus.NotUsed, "Không sử dụng" },
            { (int)AssetStatus.Waiting, "Chờ sử dụng" },
            { (int)AssetStatus.InUse, "Đang sử dụng" },
            { (int)AssetStatus.Scrap, "Phế" },
            { (int)AssetStatus.Liquidated, "Thanh lý" },
        };

        // ─── Tính khoảng thời gian theo kỳ ───
        private (DateTime From, DateTime To, string Label) ResolvePeriod(AssetReportSearchDto dto)
        {
            switch (dto.PeriodType)
            {
                case ReportPeriodType.Month:
                    {
                        var month = dto.Month ?? throw new Exception("Vui lòng chọn tháng");
                        var from = new DateTime(dto.Year, month, 1);
                        var to = from.AddMonths(1).AddTicks(-1);
                        return (from, to, $"Tháng {month}/{dto.Year}");
                    }
                case ReportPeriodType.Quarter:
                    {
                        var quarter = dto.Quarter ?? throw new Exception("Vui lòng chọn quý");
                        var startMonth = (quarter - 1) * 3 + 1;
                        var from = new DateTime(dto.Year, startMonth, 1);
                        var to = from.AddMonths(3).AddTicks(-1);
                        return (from, to, $"Quý {quarter}/{dto.Year}");
                    }
                case ReportPeriodType.Year:
                default:
                    {
                        var from = new DateTime(dto.Year, 1, 1);
                        var to = from.AddYears(1).AddTicks(-1);
                        return (from, to, $"Năm {dto.Year}");
                    }
            }
        }

        public async Task<AssetReportResultDto> GetReportAsync(AssetReportSearchDto dto)
        {
            var (from, to, label) = ResolvePeriod(dto);

            var query = _context.Assets_Information
                .AsNoTracking()
                .Include(x => x.Model)
                .Where(x => !x.Is_Deleted && x.CreatedAt >= from && x.CreatedAt <= to);

            if (dto.BigTypeId.HasValue)
                query = query.Where(x => x.Model.Big_Type_Id == dto.BigTypeId.Value);

            if (dto.MiddleTypeId.HasValue)
                query = query.Where(x => x.Model.Middle_Type_Id == dto.MiddleTypeId.Value);

            if (dto.SmallTypeId.HasValue)
                query = query.Where(x => x.Model.Small_Type_Id == dto.SmallTypeId.Value);

            if (dto.ModelId.HasValue)
                query = query.Where(x => x.Model_Id == dto.ModelId.Value);

            if (!string.IsNullOrEmpty(dto.DepartmentId))
                query = query.Where(x => x.UseDepartmentId == dto.DepartmentId);

            var raw = await query
                .GroupBy(x => new { x.UseDepartmentId, x.Status })
                .Select(g => new
                {
                    DepartmentId = g.Key.UseDepartmentId,
                    Status = g.Key.Status,
                    Count = g.Count()
                })
                .ToListAsync();

            var departmentIds = raw.Select(x => x.DepartmentId).Distinct().ToList();
            var departments = await _context.Departments
                .AsNoTracking()
                .Where(d => departmentIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name);

            var pivot = raw
                .GroupBy(x => x.DepartmentId)
                .Select(g => new AssetReportByDepartmentDto
                {
                    DepartmentId = g.Key,
                    DepartmentName = departments.TryGetValue(g.Key, out var name) ? name : g.Key,
                    NotUsedCount = g.Where(x => x.Status == (int)AssetStatus.NotUsed).Sum(x => x.Count),
                    WaitingCount = g.Where(x => x.Status == (int)AssetStatus.Waiting).Sum(x => x.Count),
                    InUseCount = g.Where(x => x.Status == (int)AssetStatus.InUse).Sum(x => x.Count),
                    ScrapCount = g.Where(x => x.Status == (int)AssetStatus.Scrap).Sum(x => x.Count),
                    LiquidatedCount = g.Where(x => x.Status == (int)AssetStatus.Liquidated).Sum(x => x.Count),
                    TotalCount = g.Sum(x => x.Count)
                })
                .OrderBy(x => x.DepartmentName)
                .ToList();

            return new AssetReportResultDto
            {
                PeriodLabel = label,
                Items = pivot,
                GrandTotal = pivot.Sum(x => x.TotalCount)
            };
        }

        public async Task<byte[]> ExportExcelAsync(AssetReportSearchDto dto)
        {
            var report = await GetReportAsync(dto);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Báo cáo tài sản");

            // Tiêu đề
            ws.Cell(1, 1).Value = $"BÁO CÁO THỐNG KÊ TÀI SẢN - {report.PeriodLabel}";
            ws.Range(1, 1, 1, 7).Merge().Style
                .Font.SetBold()
                .Font.SetFontSize(14)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Header bảng
            string[] headers = { "STT", "Đơn vị", "Không sử dụng", "Chờ sử dụng", "Đang sử dụng", "Phế", "Thanh lý", "Tổng cộng" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(3, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.SetBold();
                cell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            }

            int row = 4;
            int stt = 1;
            foreach (var item in report.Items)
            {
                ws.Cell(row, 1).Value = stt++;
                ws.Cell(row, 2).Value = item.DepartmentName;
                ws.Cell(row, 3).Value = item.NotUsedCount;
                ws.Cell(row, 4).Value = item.WaitingCount;
                ws.Cell(row, 5).Value = item.InUseCount;
                ws.Cell(row, 6).Value = item.ScrapCount;
                ws.Cell(row, 7).Value = item.LiquidatedCount;
                ws.Cell(row, 8).Value = item.TotalCount;

                ws.Range(row, 1, row, 8).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                row++;
            }

            // Dòng tổng cộng
            ws.Cell(row, 2).Value = "TỔNG CỘNG";
            ws.Cell(row, 2).Style.Font.SetBold();
            ws.Cell(row, 3).FormulaA1 = $"=SUM(C4:C{row - 1})";
            ws.Cell(row, 4).FormulaA1 = $"=SUM(D4:D{row - 1})";
            ws.Cell(row, 5).FormulaA1 = $"=SUM(E4:E{row - 1})";
            ws.Cell(row, 6).FormulaA1 = $"=SUM(F4:F{row - 1})";
            ws.Cell(row, 7).FormulaA1 = $"=SUM(G4:G{row - 1})";
            ws.Cell(row, 8).FormulaA1 = $"=SUM(H4:H{row - 1})";
            ws.Range(row, 1, row, 8).Style.Font.SetBold();
            ws.Range(row, 1, row, 8).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}