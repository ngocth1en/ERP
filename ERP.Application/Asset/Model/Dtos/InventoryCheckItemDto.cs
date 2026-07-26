namespace ERP.Application.Asset.Model.Dtos
{
    public class InventoryCheckItemDto
    {
        // gi? tr? QR ho?c AssetNo do client g?i
        public string Code { get; set; } = default!;
        // ghi ch? n?u ph?t hi?n h? h?ng
        public string? Note { get; set; }
        // optional flag t? UI ?? ??nh d?u h?/h?ng
        public bool IsDamaged { get; set; }
    }
}