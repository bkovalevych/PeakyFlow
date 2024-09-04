namespace PeakyFlow.Abstractions
{
    public abstract class PlayerBase : Entity
    {
        public required string Name { get; set; }
    }


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
        StockAction StockAction,
        bool IsStock,
        bool IsBusiness,
        bool IsRealEstate
        )
    {
        public static Card GetDefault()
        {
            return new Card("", "", "", CardType.Default, false, "", "", "", "", "", 0, 0, 0, 0, StockAction.Default, false, false, false);
        }
    }

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

    public enum StepType
    {
        Start,
        Salary,
        Children,
        Charity,
        Market,
        Deal,
        MoneyToTheWind,
        Downsize,
        DownsizeWait
    }
}
