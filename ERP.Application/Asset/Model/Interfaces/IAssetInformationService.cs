using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Common;
using ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetInformationService
    {
        Task<PagedResult<AssetStatisticResultDto>> SearchStatisticAsync(
            AssetStatisticSearchDto searchDto);
        Task<PagedResult<AssetByModelResultDto>> SearchAssetsByModelAsync(
    AssetByModelSearchDto dto);
        Task CreateAsync(CreateAssetDto dto, string username);
        Task UpdateAsync(string assetNo, UpdateAssetDto dto, string username);
    }
}
