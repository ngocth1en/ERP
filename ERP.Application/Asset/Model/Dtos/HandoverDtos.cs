using ERP.Domain.Enum.ASM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    // ─── REQUEST ──────────────────────────────────────────────

    public class CreateHandoverDto
    {
        public string FromDepartmentId { get; set; } = null!;
        public string ToDepartmentId { get; set; } = null!;
        public string? FromStorageLocationNo { get; set; }
        public string? ToStorageLocationNo { get; set; }
        public string? Note { get; set; }
        /// <summary>Danh sách AssetNo kèm theo khi tạo đơn</summary>
        public List<string> AssetNos { get; set; } = new();
    }

    public class UpdateHandoverDto
    {
        public string? ToDepartmentId { get; set; }
        public string? FromStorageLocationNo { get; set; }
        public string? ToStorageLocationNo { get; set; }
        public string? Note { get; set; }
    }

    public class AddHandoverAssetsDto
    {
        /// <summary>Thêm 1 hoặc nhiều tài sản vào phiếu</summary>
        public List<string> AssetNos { get; set; } = new();
    }

    public class SearchHandoverAssetsDto
    {
        public string DepartmentId { get; set; } = null!;
        public string? StorageLocationNo { get; set; }
        public string? AssetNo { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class SearchHandoverOrdersDto
    {
        public string? FromDepartmentId { get; set; }
        public string? ToDepartmentId { get; set; }
        public HandoverStatus? Status { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // ─── RESPONSE ─────────────────────────────────────────────

    public class HandoverOrderDto
    {
        public string Id { get; set; } = null!;
        public string FromDepartmentId { get; set; } = null!;
        public string FromDepartmentName { get; set; } = null!;
        public string ToDepartmentId { get; set; } = null!;
        public string ToDepartmentName { get; set; } = null!;
        public string? FromStorageLocationNo { get; set; }
        public string? ToStorageLocationNo { get; set; }
        public HandoverStatus Status { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiredAt => CreatedAt.AddDays(3);
        public string? CreatedByUserName { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? ConfirmedByUserName { get; set; }
        public List<HandoverDetailDto> Details { get; set; } = new();
    }

    public class HandoverOrderListDto
    {
        public string Id { get; set; } = null!;
        public string FromDepartmentId { get; set; } = null!;
        public string FromDepartmentName { get; set; } = null!;
        public string ToDepartmentId { get; set; } = null!;
        public string ToDepartmentName { get; set; } = null!;
        public HandoverStatus Status { get; set; }
        public int AssetCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserName { get; set; }
    }

    public class HandoverDetailDto
    {
        public int Id { get; set; }
        public string AssetNo { get; set; } = null!;
        public string? AssetName { get; set; }
        public string? ModelName { get; set; }
        public string PreviousUseDepartmentId { get; set; } = null!;
        public string PreviousStorageLocationNo { get; set; } = null!;
        public string? Note { get; set; }
    }

    public class HandoverAssetCandidateDto
    {
        public string AssetNo { get; set; } = null!;
        public string AssetName { get; set; } = null!;
        public string ModelName { get; set; } = null!;
        public string StorageLocationNo { get; set; } = null!;
        public string UseDepartmentId { get; set; } = null!;
        public int Status { get; set; }
    }
}
