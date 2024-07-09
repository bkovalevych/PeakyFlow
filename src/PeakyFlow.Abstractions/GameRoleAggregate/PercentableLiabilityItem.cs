namespace PeakyFlow.Abstractions.GameRoleAggregate
{
    public record PercentableLiabilityItem(
        string Id, 
        string Name, 
        FinancialType FinancialType, 
        int LiabilityAmount, 
        int Percent) 
        : FinancialItemBase(
            Id, 
            Name, 
            FinancialType, 
            0, 
            LiabilityAmount, 
            (int)Math.Round(-LiabilityAmount * Percent / 100f, 0));

}
