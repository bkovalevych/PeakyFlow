namespace PeakyFlow.Abstractions.GameRoleAggregate
{
    public record FinancialItem(
        string Id, 
        string Name, 
        FinancialType FinancialType, 
        int AssetAmount, 
        int LiabilityAmount, 
        int FlowAmount)
        : FinancialItemBase(
            Id, 
            Name, 
            FinancialType, 
            AssetAmount, 
            LiabilityAmount, 
            FlowAmount);
}
