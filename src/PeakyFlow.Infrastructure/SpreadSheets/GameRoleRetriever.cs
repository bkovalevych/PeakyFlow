using PeakyFlow.Abstractions.GameRoleAggregate;

namespace PeakyFlow.Infrastructure.SpreadSheets
{
    internal class GameRoleRetriever : ISheetsRetriever<GameRole>
    {
        public string Range => "'Proffessions'!A1:Q3";

        public static T ToValue<T>(string rawVal, T defaultValue = default)
            where T : struct
        {
            if (rawVal != null && typeof(T).IsEnum)
            {
                Enum.TryParse<T>(rawVal, true, out var result);
                return result;
            }
            else if (rawVal != null && TryConvertValue(rawVal, out T convertedValue))
            {
                return convertedValue;
            }
            return defaultValue;
        }

        private static bool TryConvertValue<T>(string value, out T convertedValue)
            where T : struct
        {
            try
            {
                convertedValue = (T)Convert.ChangeType(value, typeof(T));
                return true;
            }
            catch (Exception)
            {
                convertedValue = default;
                return false;
            }
        }

        public List<GameRole> Retrieve(IList<IList<object>> objects)
        {
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
                var income = ToValue(row[names["Income"]].ToString() ?? string.Empty, 0);
                var taxes = ToValue(row[names["Taxes"]].ToString() ?? string.Empty, 0);
                var homeMortgage = ToValue(row[names["Home Mortgage"]].ToString() ?? string.Empty, 0);
                var schoolExpenses = ToValue(row[names["School Expenses"]].ToString() ?? string.Empty, 0);
                var carPayment = ToValue(row[names["Car Payment"]].ToString() ?? string.Empty, 0);
                var creditCards = ToValue(row[names["Credit Cards"]].ToString() ?? string.Empty, 0);
                var retailPayment = ToValue(row[names["Retail Payment"]].ToString() ?? string.Empty, 0);
                var otherExpenses = ToValue(row[names["Other Expenses"]].ToString() ?? string.Empty, 0);
                var childExpenses = ToValue(row[names["Child Expenses"]].ToString() ?? string.Empty, 0);

                var savings = ToValue(row[names["Savings"]].ToString() ?? string.Empty, 0);
                var mortgageLiability = ToValue(row[names["Mortgage Liability"]].ToString() ?? string.Empty, 0);
                var schoolLoan = ToValue(row[names["School Loans"]].ToString() ?? string.Empty, 0);
                var carLoan = ToValue(row[names["Car Loans"]].ToString() ?? string.Empty, 0);
                var creditCardsLiability = ToValue(row[names["Credit cards Liability"]].ToString() ?? string.Empty, 0);
                var retailDebt = ToValue(row[names["Retail debt"]].ToString() ?? string.Empty, 0);

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
