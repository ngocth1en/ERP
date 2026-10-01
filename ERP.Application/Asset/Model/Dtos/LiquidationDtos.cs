using ERP.Domain.Enum.ASM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    // ─── REQUEST ──────────────────────────────────────────────

    public class CreateLiquidationDto
    {
        public string DepartmentId { get; set; } = null!;
        public string? LiquidationMethod { get; set; }
        public decimal? RecoveredAmount { get; set; }
        public string? Note { get; set; }
        public List<string> AssetNos { get; set; } = new();
    }

    public class UpdateLiquidationDto
    {
        public string? LiquidationMethod { get; set; }
        public decimal? RecoveredAmount { get; set; }
        public string? Note { get; set; }
    }

    public class AddLiquidationAssetsDto
    {
        public List<string> AssetNos { get; set; } = new();
    }

    public class SearchLiquidationOrdersDto
    {
        public string? DepartmentId { get; set; }
        public LiquidationStatus? Status { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // ─── RESPONSE ─────────────────────────────────────────────

    public class LiquidationOrderDto
    {
        public string Id { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public LiquidationStatus Status { get; set; }
        public string? LiquidationMethod { get; set; }
        public decimal? RecoveredAmount { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? ConfirmedByUserName { get; set; }
        public List<LiquidationDetailDto> Details { get; set; } = new();
    }

    public class LiquidationOrderListDto
    {
        public string Id { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public LiquidationStatus Status { get; set; }
        public int AssetCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserName { get; set; }
    }

    public class LiquidationDetailDto
    {
        public int Id { get; set; }
        public string AssetNo { get; set; } = null!;
        public string? AssetName { get; set; }
        public string? ModelName { get; set; }
        public string? ScrapId { get; set; }
        public string PreviousUseDepartmentId { get; set; } = null!;
        public string? Note { get; set; }
    }
}
