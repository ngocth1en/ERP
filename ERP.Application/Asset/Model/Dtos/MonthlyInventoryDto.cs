namespace ERP.Application.Asset.Model.Dtos
{
    public class MonthlyInventoryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int ExpectedCount { get; set; }
        public int Counted { get; set; }
        public int MissingCount { get; set; }
        public int UnexpectedCount { get; set; }

        public string YearMonth => $"{Year:D4}-{Month:D2}";
    }
}