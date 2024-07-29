using MediatR;
using PeakyFlow.Abstractions.GameRoleAggregate;

namespace PeakyFlow.Abstractions.RoomStateAggregate.Events
{
    public record PlayerStateCreatedEvent(
       bool ShouldTakeTurn,
       string Id,
       string Name,
       string RoomId,
       int Savings,
       bool IsBankrupt,
       string RoleName,
       string Description,
       string? ImageId,
       IEnumerable<FinancialItemBase> CountableLiabilities,
       IEnumerable<FinancialItemBase> PercentableLiabilities,
       IEnumerable<FinancialItemBase> Stocks,
       IEnumerable<FinancialItemBase> FinancialItems,
       int Salary,
       int Expenses,
       int Income,
       int CashFlow,
       int ExpensesForOneChild) : INotification;
}
