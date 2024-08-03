namespace PeakyFlow.Abstractions.GameRoleAggregate
{
    public record StockItem(
        string Id,
        string Name, 
        FinancialType FinancialType, 
        int Count, 
        int PriceForOne,
        int FlowForOne = 0,
        string? Group = null) 
        : FinancialItemBase(
            Id,
            Name,
            FinancialType,
            PriceForOne * Count, 
            0, 
            FlowForOne * Count,
            Group);
}
