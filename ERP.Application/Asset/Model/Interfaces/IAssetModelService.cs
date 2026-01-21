using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetModelService
    {
        Task<PagedResult<AssetModelListDto>> SearchAsync(
            AssetModelSearchDto dto);
        Task<int> CreateAsync(CreateAssetModelDto dto, string username);
        Task UpdateAsync(int id, UpdateAssetModelDto dto, string username);
    }
}
