using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetMiddleTypeService
    {
        Task<PagedResult<AssetMiddleTypeListDto>> SearchAsync(AssetMiddleTypeSearchDto dto);
        Task<int> CreateAsync(CreateAssetMiddleTypeDto dto, Guid userId);
        Task UpdateAsync(int id, UpdateAssetMiddleTypeDto dto, Guid userId);
        Task ToggleActiveAsync(int id, Guid userId);
    }
}
