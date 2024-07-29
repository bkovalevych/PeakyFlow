namespace PeakyFlow.Abstractions.GameRoleAggregate
{
    public abstract record FinancialItemBase(
        string Name, 
        FinancialType FinancialType, 
        int AssetAmount, 
        int LiabilityAmount, 
        int FlowAmount,
        string? Group);
}
