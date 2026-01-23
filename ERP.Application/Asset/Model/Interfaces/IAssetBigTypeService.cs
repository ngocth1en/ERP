using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetBigTypeService
    {
        Task<PagedResult<AssetBigTypeListDto>> SearchAsync(SearchAssetBigTypeDto dto);
        Task CreateAsync(CreateAssetBigTypeDto dto, string userName);
        Task UpdateAsync(int id, UpdateAssetBigTypeDto dto, string userName);
        Task DeleteAsync(int id, string userName);
        Task<List<LookupDto>> GetBigTypeAsync();
    }

}
