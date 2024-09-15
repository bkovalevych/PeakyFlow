using PeakyFlow.Abstractions.GameRoleAggregate;

namespace PeakyFlow.Application.RoomStates
{
    public record PlayerStateDto(
        string Id,
        string Name,
        string RoomId,
        int Savings,
        bool IsBankrupt,
        IEnumerable<FinancialItemBase> CountableLiabilities,
        IEnumerable<FinancialItemBase> PercentableLiabilities,
        IEnumerable<FinancialItemBase> Stocks,
        IEnumerable<FinancialItemBase> FinancialItems,
        int Salary,
        int Expenses,
        int Income,
        int CashFlow,
        float PercentageToWin,
        bool HasWon,
        bool HasLost,
        int ExpensesForOneChild);
}
