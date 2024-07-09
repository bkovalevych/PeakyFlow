namespace PeakyFlow.Abstractions.GameRoleAggregate
{
    public class GameRole : Entity, IAggregateRoot
    {
        private IEnumerable<FinancialItemBase> Flows => Enumerable.Empty<FinancialItemBase>()
            .Union(CountableLiabilities)
            .Union(PercentableLiabilities)
            .Union(Stocks)
            .Union(FinancialItems);

        public required string RoleName { get; set; }

        public required string Description { get; set; }

        public string? ImageId { get; set; }

        public IEnumerable<CountableLiabilityItem> CountableLiabilities { get; set; } = [];
        public IEnumerable<PercentableLiabilityItem> PercentableLiabilities { get; set; } = [];
        public IEnumerable<StockItem> Stocks { get; set; } = [];
        public IEnumerable<FinancialItem> FinancialItems { get; set; } = [];

        public int Salary => Flows.Where(x => x.FinancialType == FinancialType.Salary).Sum(x => x.FlowAmount);
        public int Expenses => Flows.Where(x => x.FlowAmount < 0).Sum(x => -x.FlowAmount);
        public int Income => Flows.Where(x => x.FlowAmount > 0).Sum(x => x.FlowAmount);
        public int CashFlow => Flows.Sum(x => x.FlowAmount);
        public int InitialSavings => Flows.Where(x => x.FinancialType == FinancialType.Savings).Sum(x => x.AssetAmount);
        public int ExpensesForOneChild => CountableLiabilities.Where(x => x.FinancialType == FinancialType.ChildrenExpenses).Sum(x => x.PriceForOne);
    }
}
