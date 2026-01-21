using ERP.Application.Asset.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetInformationService
    {
        Task<List<AssetStatisticResultDto>> SearchStatisticAsync(
            AssetStatisticSearchDto searchDto);
    }
}
