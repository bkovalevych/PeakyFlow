namespace PeakyFlow.Abstractions.GameRoleAggregate
{
    public record FinancialItem(
        string Name, 
        FinancialType FinancialType, 
        int AssetAmount, 
        int LiabilityAmount, 
        int FlowAmount,
        string? Group = null)
        : FinancialItemBase(
            Name, 
            FinancialType, 
            AssetAmount, 
            LiabilityAmount, 
            FlowAmount, 
            Group);
}
