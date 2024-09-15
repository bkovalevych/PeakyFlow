namespace PeakyFlow.Abstractions.GameRoleAggregate
{
    public class GameRole : Entity, IAggregateRoot
    {
        public GameRole() { }

        public GameRole(
            string name,
            string description,
            int childExpenses,
            int savings,
            int income,
            int taxes,
            int mortgageLiability,
            int homeMortgage,
            int schoolLoan,
            int schoolExpenses,
            int carLoan,
            int carPayment,
            int creditCardsLiability,
            int creditCards,
            int retailDebt,
            int retailPayment,
            int otherExpenses)
        {
            RoleName = name;
            Description = description;
            CountableLiabilities = [
                new CountableLiabilityItem("Children", "Children", FinancialType.ChildrenExpenses, 0, childExpenses)];
            FinancialItems =
            [
                    new FinancialItem("savings", "Savings", FinancialType.Savings, savings, 0, 0),
                    new FinancialItem("salary", "Salary", FinancialType.Salary, 0, 0, income),
                    new FinancialItem("taxes", "Taxes", FinancialType.Taxes, 0, 0, -taxes),
                    new FinancialItem("home mortgage", "Home Mortgage", FinancialType.Loan, 0, mortgageLiability, -homeMortgage),
                    new FinancialItem("school", "School", FinancialType.Loan, 0, schoolLoan, -schoolExpenses),
                    new FinancialItem("car", "Car", FinancialType.Loan, 0, carLoan, -carPayment),
                    new FinancialItem("credit cards", "Credit Cards", FinancialType.Loan, 0, creditCardsLiability, -creditCards),
                    new FinancialItem("retail payment", "Retail Payment", FinancialType.Loan, 0, retailDebt, -retailPayment),
                    new FinancialItem("other expenses", "Other Expenses", FinancialType.Others, 0, 0, -otherExpenses)
            ];
        }

        private IEnumerable<FinancialItemBase> Flows => Enumerable.Empty<FinancialItemBase>()
            .Union(CountableLiabilities)
            .Union(PercentableLiabilities)
            .Union(Stocks)
            .Union(FinancialItems);

        public string RoleName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

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
