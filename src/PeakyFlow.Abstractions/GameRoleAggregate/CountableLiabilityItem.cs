namespace PeakyFlow.Abstractions.GameRoleAggregate
{
    public record CountableLiabilityItem(
        string Name, 
        FinancialType FinancialType, 
        int Count, 
        int PriceForOne,
        int LiabilityForOne = 0,
        string? Group = null) 
        : FinancialItemBase(Name, FinancialType, 0, LiabilityForOne * Count, -PriceForOne * Count, Group);
}
