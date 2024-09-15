namespace PeakyFlow.Infrastructure.SpreadSheets
{
    public class SheetsSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string SheetId { get; set; } = string.Empty;
        public string BigDealsRange { get; set; } = string.Empty;
        public string MoneyToTheWindRange { get; set; } = string.Empty;
        public string SmallDealsRange { get; set; } = string.Empty;
        public string StocksRange {  get; set; } = string.Empty;
        public string MarketRange {  get; set; } = string.Empty;
        public string GameMapRange {  get; set; } = string.Empty;
        public string GameRoleRange { get; set; } = string.Empty;
    }
}
