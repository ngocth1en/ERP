using ERP.Application.Asset.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetReportService
    {
        Task<AssetReportResultDto> GetReportAsync(AssetReportSearchDto dto);

        /// <summary>Xuất báo cáo ra file Excel, trả về mảng byte</summary>
        Task<byte[]> ExportExcelAsync(AssetReportSearchDto dto);
    }
}
