using System.Collections.Generic;

namespace ERP.Application.Asset.Model.Dtos
{
    public class InventoryCheckResultDto
    {
        public string InventoryType { get; set; } = default!; // "1" = scan, "2" = count

        public int ExpectedCount { get; set; }
        public int Counted { get; set; }
        public int MissingCount { get; set; }
        public int UnexpectedCount { get; set; }

        public List<string> MissingAssetNos { get; set; } = new();
        public List<string> UnexpectedScannedQRCodes { get; set; } = new();
        public List<string> FoundAssetNos { get; set; } = new();
    }
}