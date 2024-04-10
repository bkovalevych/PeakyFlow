using System.Text.Json;

namespace PeakyFlow.Abstractions.UnitTests
{
    public class ModelsTests
    {
        [Fact]
        public void TestFiannaces()
        {
            //Assign

            var financeInfos = new List<FinancialEntryInfo>()
            {
                new FinancialEntryInfo("1", "Children", 1, FinancialEntryInfo.FinancialType.Countable, null),
                new FinancialEntryInfo("3", "Home Mortgage", 3, FinancialEntryInfo.FinancialType.Percent, 1),
                new FinancialEntryInfo("4", "Education", 4, FinancialEntryInfo.FinancialType.Percent, 8),
                new FinancialEntryInfo("5", "Car payment", 5, FinancialEntryInfo.FinancialType.Percent, 2),
                new FinancialEntryInfo("6", "Credit card payment", 6, FinancialEntryInfo.FinancialType.Percent, 3),
                new FinancialEntryInfo("7", "Retail payment", 7, FinancialEntryInfo.FinancialType.Percent, 5),

            };

            var assetsAndLiabilities = new List<FinancialEntry>
            {
                new AssetEntry("a1", "0", "Savings", 40),
                new LiabilityEntry("l1", "1", "Children", 48),
                new LiabilityEntry("l2", "3", "Home Mortgage", 14300),
                new LiabilityEntry("l3", "4", "Education", 0),
                new LiabilityEntry("l4", "5", "Car payment", 1500),
                new LiabilityEntry("l5", "6", "Credit car payment", 2200),
                new LiabilityEntry("l6", "7", "Retail payment", 100)
            };

            var roleDetails = new PlayerRoleDetails(
                assetsAndLiabilities.First(x => x.FinancialEntryInfoId == "0").Amount,
                100,
                assetsAndLiabilities.First(x => x.FinancialEntryInfoId == "1").Amount);

            var playerState = new PlayerState("1", 100, 3, 2, 0, 3);

            var financialFlows = new List<FinancialFlowEntry>()
            {
                new IncomeFlowEntry("Salary", 950),
                new ExpenseFlowEntry("Taxes", 235),
                new ExpenseFlowEntry("Other expenses", 221)
            };

            var countableFlows = financeInfos
                .Join(
                    assetsAndLiabilities,
                    outer => outer.Id,
                    inner => inner.FinancialEntryInfoId,
                    (info, finance) => (info, finance))
                .Select(x => x.finance.GetFinancialFlowEntryByInfo(x.info, roleDetails, playerState))
                .Where(x => x != null && x.Amount != 0)
                .Cast<FinancialFlowEntry>();

            financialFlows.AddRange(countableFlows);

            var playerRole = new PlayerRole("1", "Lawer", null, "Lawer in law",
                roleDetails,
                assetsAndLiabilities.Where(x => x is AssetEntry).Cast<AssetEntry>(),
                assetsAndLiabilities.Where(x => x is LiabilityEntry).Cast<LiabilityEntry>(),
                financialFlows.Where(x => x is IncomeFlowEntry).Cast<IncomeFlowEntry>(),
                financialFlows.Where(x => x is ExpenseFlowEntry).Cast<ExpenseFlowEntry>());

            var json = JsonSerializer.Serialize(playerRole);
        }

        [Fact]
        public void TestAddingFiannaces()
        {
            //Assign

            var financeInfos = new List<FinancialEntryInfo>()
            {
                new FinancialEntryInfo("1", "Children", 1, FinancialEntryInfo.FinancialType.Countable, null),
                new FinancialEntryInfo("3", "Home Mortgage", 3, FinancialEntryInfo.FinancialType.Percent, 1),
                new FinancialEntryInfo("4", "Education", 4, FinancialEntryInfo.FinancialType.Percent, 8),
                new FinancialEntryInfo("5", "Car payment", 5, FinancialEntryInfo.FinancialType.Percent, 2),
                new FinancialEntryInfo("6", "Credit card payment", 6, FinancialEntryInfo.FinancialType.Percent, 3),
                new FinancialEntryInfo("7", "Retail payment", 7, FinancialEntryInfo.FinancialType.Percent, 5),
                new FinancialEntryInfo("8", "ON2U", 8, null, null),
            };

            var assetsAndLiabilities = new List<FinancialEntry>
            {
                new AssetEntry("a1", "0", "Savings", 40),
                new LiabilityEntry("l1", "1", "Children", 48),
                new LiabilityEntry("l2", "3", "Home Mortgage", 14300),
                new LiabilityEntry("l3", "4", "Education", 0),
                new LiabilityEntry("l4", "5", "Car payment", 1500),
                new LiabilityEntry("l5", "6", "Credit car payment", 2200),
                new LiabilityEntry("l6", "7", "Retail payment", 100)

            };

            var roleDetails = new PlayerRoleDetails(
                assetsAndLiabilities.First(x => x.FinancialEntryInfoId == "0").Amount,
                100,
                assetsAndLiabilities.First(x => x.FinancialEntryInfoId == "1").Amount);

            var playerState = new PlayerState("1", 100, 3, 2, 0, 3);

            var financialFlows = new List<FinancialFlowEntry>()
            {
                new IncomeFlowEntry("Salary", 950),
                new ExpenseFlowEntry("Taxes", 235),
                new ExpenseFlowEntry("Other expenses", 221)
            };

            var countableFlows = financeInfos
                .Join(
                    assetsAndLiabilities,
                    outer => outer.Id,
                    inner => inner.FinancialEntryInfoId,
                    (info, finance) => (info, finance))
                .Select(x => x.finance.GetFinancialFlowEntryByInfo(x.info, roleDetails, playerState))
                .Where(x => x != null && x.Amount != 0)
                .Cast<FinancialFlowEntry>();

            financialFlows.AddRange(countableFlows);

            var playerRole = new PlayerRole("1", "Lawer", null, "Lawer in law",
                roleDetails,
                assetsAndLiabilities.Where(x => x is AssetEntry).Cast<AssetEntry>(),
                assetsAndLiabilities.Where(x => x is LiabilityEntry).Cast<LiabilityEntry>(),
                financialFlows.Where(x => x is IncomeFlowEntry).Cast<IncomeFlowEntry>(),
                financialFlows.Where(x => x is ExpenseFlowEntry).Cast<ExpenseFlowEntry>());

            var json = JsonSerializer.Serialize(playerRole);
        }
    }
}