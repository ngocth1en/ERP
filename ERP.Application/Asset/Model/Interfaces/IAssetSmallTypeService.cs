using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetSmallTypeService
    {
        Task<PagedResult<AssetSmallTypeListDto>> SearchAsync(
            AssetSmallTypeSearchDto dto);

        Task<int> CreateAsync(
            CreateAssetSmallTypeDto dto,
            string username);

        Task UpdateAsync(
            int id,
            UpdateAssetSmallTypeDto dto,
            string username);

        Task ToggleActiveAsync(
            int id,
            string username);
        Task<List<LookupDto>> GetByMiddleTypeAsync(int middleTypeId);
    }
}
