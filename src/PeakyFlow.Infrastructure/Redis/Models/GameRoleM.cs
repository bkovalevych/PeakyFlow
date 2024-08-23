using PeakyFlow.Abstractions.GameRoleAggregate;
using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = [nameof(GameRole)])]
    public class GameRoleM : EntityM
    {
        public required string RoleName { get; set; }

        public required string Description { get; set; }

        public string? ImageId { get; set; }

        public CountableLiabilityItem[] CountableLiabilities { get; set; } = [];
        public PercentableLiabilityItem[] PercentableLiabilities { get; set; } = [];
        public StockItem[] Stocks { get; set; } = [];
        public FinancialItem[] FinancialItems { get; set; } = [];

        public int Salary { get; set; }
        public int Expenses { get; set; }
        public int Income { get; set; }
        public int CashFlow { get; set; }
        public int InitialSavings { get; set; }
        public int ExpensesForOneChild { get; set; }
    }
}
