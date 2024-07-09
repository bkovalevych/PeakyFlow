using PeakyFlow.Abstractions.GameRoleAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate;
using System.Text.Json;

namespace PeakyFlow.Abstractions.UnitTests
{
    public class ModelsTests
    {
        [Fact]
        public void TestFiannaces()
        {
            //Assign

            var financeItems = new List<FinancialItemBase>()
            {
                new FinancialItem("savings", "Savings", FinancialType.Savings, 40, 0, 0),
                new FinancialItem("salary", "Salary", FinancialType.Salary, 0, 0, 950),
                new FinancialItem("taxes", "Taxes", FinancialType.Taxes, 0, 0, -235),
                new FinancialItem("otherExpenses", "Other expenses", FinancialType.Others, 0, 0, -221),
                new CountableLiabilityItem("children", "Children Expenses", FinancialType.ChildrenExpenses, 0, 48),
                new PercentableLiabilityItem("homeMortgage", "Home Mortgage", FinancialType.Loan, 14300, 1),
                new PercentableLiabilityItem("education", "Education", FinancialType.Loan, 0, 8),
                new PercentableLiabilityItem("carPayment", "Car Payement", FinancialType.Loan, 1500, 2),
                new PercentableLiabilityItem("creditCardPayment", "Credit card payement", FinancialType.Loan, 2200, 3),
                new PercentableLiabilityItem("retailPayment", "Retail payment", FinancialType.Loan, 100, 5),


            };


            var playerState = new PlayerState() 
            {
                Id = "1",
                Description = "descr",
                Name = "boh",
                RoleName = "lawer"
            };

            
            var playerRole = new GameRole()
            {
                Id = "1",
                RoleName = "Lawer", 
                Description = "Lawer in law",
                CountableLiabilities = financeItems.Where(x => x is CountableLiabilityItem).Cast<CountableLiabilityItem>(),
                PercentableLiabilities = financeItems.Where(x => x is PercentableLiabilityItem).Cast<PercentableLiabilityItem>(),
                Stocks = financeItems.Where(x => x is StockItem).Cast<StockItem>(),
                FinancialItems = financeItems.Where(x => x is FinancialItem).Cast<FinancialItem>()
            };

            var json = JsonSerializer.Serialize(playerRole);

            var other = JsonSerializer.Deserialize<GameRole>(json);
        }
    }
}