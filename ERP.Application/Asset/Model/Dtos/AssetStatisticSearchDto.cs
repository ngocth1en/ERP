using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class AssetStatisticSearchDto
    {
        public int? BigTypeId { get; set; }
        public int? MiddleTypeId { get; set; }

        public string? DepartmentId { get; set; }

        public string? AssetNo { get; set; }

        /// <summary>
        /// 1: NotUsed
        /// 2: Waiting
        /// 3: InUse
        /// 4: Scrapt
        /// 5: Liquidated
        /// </summary>
        public int? Status { get; set; }

        public int PageIndex { get; set; } = 1; // trang số mấy
        public int PageSize { get; set; } = 10; // số dòng / trang
    }
}
