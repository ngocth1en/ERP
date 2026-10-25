using ERP.Domain.Enum.ASM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    // ─── REQUEST ──────────────────────────────────────────────

    public class CreateScrapDto
    {
        public string DepartmentId { get; set; } = null!;
        public string? StorageLocationNo { get; set; }
        public string? Reason { get; set; }
        public string? Note { get; set; }
        /// <summary>Danh sách AssetNo kèm theo khi tạo phiếu</summary>
        public List<string> AssetNos { get; set; } = new();
    }

    public class UpdateScrapDto
    {
        public string? Reason { get; set; }
        public string? Note { get; set; }
    }

    public class AddScrapAssetsDto
    {
        /// <summary>Thêm 1 hoặc nhiều tài sản vào phiếu</summary>
        public List<string> AssetNos { get; set; } = new();
    }

    public class SearchScrapAssetsDto
    {
        public string DepartmentId { get; set; } = null!;
        public string? StorageLocationNo { get; set; }
        public string? AssetNo { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class SearchScrapOrdersDto
    {
        public string? DepartmentId { get; set; }
        public ScrapStatus? Status { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // ─── RESPONSE ─────────────────────────────────────────────

    public class ScrapOrderDto
    {
        public string Id { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string? StorageLocationNo { get; set; }
        public ScrapStatus Status { get; set; }
        public string? Reason { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? ConfirmedByUserName { get; set; }
        public List<ScrapDetailDto> Details { get; set; } = new();
    }

    public class ScrapOrderListDto
    {
        public string Id { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public ScrapStatus Status { get; set; }
        public int AssetCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserName { get; set; }
    }

    public class ScrapDetailDto
    {
        public int Id { get; set; }
        public string AssetNo { get; set; } = null!;
        public string? AssetName { get; set; }
        public string? ModelName { get; set; }
        public string PreviousUseDepartmentId { get; set; } = null!;
        public string PreviousStorageLocationNo { get; set; } = null!;
        public string? Note { get; set; }
    }

    public class ScrapAssetCandidateDto
    {
        public string AssetNo { get; set; } = null!;
        public string AssetName { get; set; } = null!;
        public string ModelName { get; set; } = null!;
        public string StorageLocationNo { get; set; } = null!;
        public string UseDepartmentId { get; set; } = null!;
        public int Status { get; set; }
    }
}
