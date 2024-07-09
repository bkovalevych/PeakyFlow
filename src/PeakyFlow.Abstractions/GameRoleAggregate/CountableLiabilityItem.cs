namespace PeakyFlow.Abstractions.GameRoleAggregate
{
    public record CountableLiabilityItem(
        string Id, 
        string Name, 
        FinancialType FinancialType, 
        int Count, 
        int PriceForOne) 
        : FinancialItemBase(Id, Name, FinancialType, 0, 0, -PriceForOne * Count);
}
