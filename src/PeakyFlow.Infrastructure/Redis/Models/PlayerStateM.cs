using PeakyFlow.Abstractions.GameRoleAggregate;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    public class PlayerStateM : PlayerBaseM
    {
        public int Savings { get; set; }

        public float PercentageToWin { get; set; }

        public bool IsBankrupt { get; set; }

        public required string RoleName { get; set; }

        public required string Description { get; set; }

        public string? ImageId { get; set; }

        public List<CountableLiabilityItem> CountableLiabilities { get; set; } = [];
        public List<PercentableLiabilityItem> PercentableLiabilities { get; set; } = [];
        public List<StockItem> Stocks { get; set; } = [];
        public List<FinancialItem> FinancialItems { get; set; } = [];
    }
}
