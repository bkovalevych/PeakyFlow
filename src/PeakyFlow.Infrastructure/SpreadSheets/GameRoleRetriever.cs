using Microsoft.Extensions.Options;
using PeakyFlow.Abstractions.Common.Interfaces;
using PeakyFlow.Abstractions.GameRoleAggregate;

namespace PeakyFlow.Infrastructure.SpreadSheets
{
    internal class GameRoleRetriever(IStringConverter stringConverter, IOptions<SheetsSettings> options) : ISheetsRetriever<GameRole>
    {
        public List<string> Ranges => [options.Value.GameRoleRange];

        public List<GameRole> Retrieve(IList<IList<IList<object>>> ranges)
        {
            var objects = ranges[0];
            var names = objects[0]
                .Select((name, index) => (name: name?.ToString() ?? string.Empty, index))
                .ToDictionary(x => x.name, x => x.index);
            // Id	Name	Income	Taxes	
            // Home Mortgage	
            // School Expenses	
            // Car Payment	
            // Credit Cards	
            // Retail Payment	
            // Other Expenses	
            // Child Expenses	
            // Savings	
            // Mortgage Liability	
            // School Loans	
            // Car Loans	
            // Credit cards Liability	
            // Retail debt

            var roles = new List<GameRole>();

            foreach (var row in objects.Skip(1))
            {
                var id = row[names["Id"]].ToString() ?? string.Empty;
                var name = row[names["Name"]].ToString() ?? string.Empty;
                var description = row[names["Name"]].ToString() ?? string.Empty;
                var income = stringConverter.ToValue(row[names["Income"]].ToString() ?? string.Empty, 0);
                var taxes = stringConverter.ToValue(row[names["Taxes"]].ToString() ?? string.Empty, 0);
                var homeMortgage = stringConverter.ToValue(row[names["Home Mortgage"]].ToString() ?? string.Empty, 0);
                var schoolExpenses = stringConverter.ToValue(row[names["School Expenses"]].ToString() ?? string.Empty, 0);
                var carPayment = stringConverter.ToValue(row[names["Car Payment"]].ToString() ?? string.Empty, 0);
                var creditCards = stringConverter.ToValue(row[names["Credit Cards"]].ToString() ?? string.Empty, 0);
                var retailPayment = stringConverter.ToValue(row[names["Retail Payment"]].ToString() ?? string.Empty, 0);
                var otherExpenses = stringConverter.ToValue(row[names["Other Expenses"]].ToString() ?? string.Empty, 0);
                var childExpenses = stringConverter.ToValue(row[names["Child Expenses"]].ToString() ?? string.Empty, 0);

                var savings = stringConverter.ToValue(row[names["Savings"]].ToString() ?? string.Empty, 0);
                var mortgageLiability = stringConverter.ToValue(row[names["Mortgage Liability"]].ToString() ?? string.Empty, 0);
                var schoolLoan = stringConverter.ToValue(row[names["School Loans"]].ToString() ?? string.Empty, 0);
                var carLoan = stringConverter.ToValue(row[names["Car Loans"]].ToString() ?? string.Empty, 0);
                var creditCardsLiability = stringConverter.ToValue(row[names["Credit cards Liability"]].ToString() ?? string.Empty, 0);
                var retailDebt = stringConverter.ToValue(row[names["Retail debt"]].ToString() ?? string.Empty, 0);

                var role = new GameRole(
                    name,
                    description,
                    childExpenses,
                    savings,
                    income,
                    taxes,
                    mortgageLiability,
                    homeMortgage,
                    schoolLoan,
                    schoolExpenses,
                    carLoan,
                    carPayment,
                    creditCardsLiability,
                    creditCards,
                    retailDebt,
                    retailPayment,
                    otherExpenses)
                {
                    Id = id
                };


                roles.Add(role);
            }

            return roles;
        }
    }
}
