using PeakyFlow.Abstractions.GameRoleAggregate;

namespace PeakyFlow.Abstractions.RoomStateAggregate
{
    public class PlayerState : PlayerBase
    {
        public int Savings { get; set; } //InitialSavings + Flows.Sum(x => x.AssetAmount);

        public float PercentageToWin => Expenses == 0 ? 1.0f : (Income - Salary) / Expenses * 1.0f;

        private IEnumerable<FinancialItemBase> Flows => Enumerable.Empty<FinancialItemBase>()
            .Union(CountableLiabilities)
            .Union(PercentableLiabilities)
            .Union(Stocks)
            .Union(FinancialItems);

        public bool IsBankrupt => CashFlow < 0 && Savings < 0;
        public required string RoleName { get; set; }

        public required string Description { get; set; }

        public string? ImageId { get; set; }
        
        public List<CountableLiabilityItem> CountableLiabilities { get; set; } = [];
        public List<PercentableLiabilityItem> PercentableLiabilities { get; set; } = [];
        public List<StockItem> Stocks { get; set; } = [];
        public List<FinancialItem> FinancialItems { get; set; } = [];

        public int Salary => Flows.Where(x => x.FinancialType == FinancialType.Salary).Sum(x => x.FlowAmount);
        public int Expenses => Flows.Where(x => x.FlowAmount < 0).Sum(x => -x.FlowAmount);
        public int Income => Flows.Where(x => x.FlowAmount > 0).Sum(x => x.FlowAmount);
        public int CashFlow => Flows.Sum(x => x.FlowAmount);
        public int InitialSavings => Flows.Where(x => x.FinancialType == FinancialType.Savings).Sum(x => x.AssetAmount);
        public int ExpensesForOneChild => CountableLiabilities.Where(x => x.FinancialType == FinancialType.ChildrenExpenses).Sum(x => x.PriceForOne);
        public bool HasWon => Expenses <= Income - Salary;

        public bool HasLost => IsBankrupt && Stocks.Count == 0 && !FinancialItems.Any(x => x.AssetAmount > 0);
    }
}
