namespace PeakyFlow.Abstractions
{
    public record PlayerBase(string Id, string Name);
    
    

    

    public record PlayerInRoom(string Id, string Name, bool IsOnline, 
        bool IsTakingTurn, int PassiveIncomePercent) 
        : PlayerBase(Id, Name);

    public record CurrenPlayerInRoom(PlayerInRoom PlayerInfo, PlayerRole Role, PlayerState State);

    public record PlayerState(string PalyerId, int Savings, int ShouldSkeepTurns, int CanRaiseTwoDices, int Children, int Place);

    public record PlayerRole(string Id, string Name, string? ImageId, string Description, 
        IEnumerable<CountableLiabilityItem> CountableLiabilities, 
        IEnumerable<PercentableLiabilityItem> PercentableLiabilities,
        IEnumerable<StockItem> Stocks,
        IEnumerable<FinancialItem> FinancialItems)
    {
        private IEnumerable<FinancialItemBase> Flows => new List<FinancialItemBase>()
            .Union(CountableLiabilities)
            .Union(PercentableLiabilities)
            .Union(Stocks)
            .Union(FinancialItems);

        public PlayerRoleDetails PlayerRoleDetails => new PlayerRoleDetails(
            Flows.Where(x => x.FinancialType == FinancialType.Salary).Sum(x => x.FlowAmount),
            Flows.Where(x => x.FlowAmount < 0).Sum(x => -x.FlowAmount),
            Flows.Where(x => x.FlowAmount > 0).Sum(x => x.FlowAmount),
            Flows.Sum(x => x.FlowAmount),
            Flows.Where(x => x.FinancialType == FinancialType.Savings).Sum(x => x.AssetAmount),
            CountableLiabilities.Where(x => x.FinancialType == FinancialType.ChildrenExpenses).Sum(x => x.PriceForOne));

        public bool IsBankrupt => PlayerRoleDetails.CashFlow < 0; 
    }

    public record PlayerRoleDetails(int Salary, int Expenses, int Income, int CashFlow, int InitialSavings, int ExpensesForOneChild);

    

    public enum FinancialType
    {
        Others,
        Savings,
        Salary,
        ChildrenExpenses,
        Stock,
        Business,
        RealEstate,
        Loan,
        Taxes
    }

    public abstract record FinancialItemBase(string Id, string Name, FinancialType FinancialType, int AssetAmount, int LiabilityAmount, int FlowAmount);

    public record FinancialItem(string Id, string Name, FinancialType FinancialType, int AssetAmount, int LiabilityAmount, int FlowAmount)
        : FinancialItemBase(Id, Name, FinancialType, AssetAmount, LiabilityAmount, FlowAmount);

    public record CountableLiabilityItem(string Id, string Name, FinancialType FinancialType, int Count, int PriceForOne) : FinancialItemBase(Id, Name, FinancialType, 0, 0, - PriceForOne * Count);

    public record PercentableLiabilityItem(string Id, string Name, FinancialType FinancialType, int LiabilityAmount, int Percent) : FinancialItemBase(Id, Name, FinancialType, 0, LiabilityAmount, (int)Math.Round(- LiabilityAmount * Percent / 100f, 0));

    public record StockItem(string Id, string Name, FinancialType FinancialType, int Count, int PriceForOne) : FinancialItemBase(Id, Name, FinancialType, 0, 0, PriceForOne * Count);

    public record RoomInfo(string Id, string Name, string? Password);

    public record Card(
        string Id, 
        string Name,
        string Group,
        CardType CardType,
        bool Required,
        string Condition,
        string Header,
        string Description,
        string Footer,
        string TradingRange,
        int Cost,
        int Mortgage, 
        int DownPay,
        int CashFlow,
        StockAction StockAction 
        );

    public enum CardType
    {
        Default,
        BigDeal,
        SmallDeal,
        MoneyToTheWind,
        Market
    }

    public enum StockAction
    {
        Default,
        ReverseSplit1For2,
        Split2For1
    }

    public record Map (string Name, IEnumerable<Race> Races);

    public record Race (
        string Id, 
        string Name, 
        IEnumerable<Step> Steps,
        int Order);

    public record PlayerInMap(string Id, string Name, string RaceId, int Index);

    public record Step(int Index, StepType StepType);

    public enum StepType
    {
        Start,
        Salary,
        Children,
        Charity,
        Market,
        Deal,
        MoneyToTheWind,
        Downsize
    } 
}
