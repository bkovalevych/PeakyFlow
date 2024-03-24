namespace PeakyFlow.Abstractions
{
    public record PlayerBase(string Id, string Name);
    
    public record PlayerInLobby(string Id, string Name, 
        string LobbyId, bool IsReady) 
        : PlayerBase(Id, Name);

    public record LobbyInfo(string Id, string Name, 
        int NumberOfPlayers,  int NumberOfPlaces, 
        string? Password, bool IsStarted = false)
    {
        public bool IsPublic => Password == null;
        public bool IsFree => NumberOfPlaces > NumberOfPlayers;
    }

    public record LobbyWithDetails(LobbyInfo Lobby, IEnumerable<PlayerInLobby> Players);

    public record PlayerInRoom(string Id, string Name, bool IsOnline, 
        bool IsTakingTurn, int PassiveIncomePercent) 
        : PlayerBase(Id, Name);

    public record CurrenPlayerInRoom(PlayerInRoom PlayerInfo, PlayerRole Role, PlayerState State);

    public record PlayerState(string PalyerId, int Savings, int ShouldSkeepTurns, int CanRaiseTwoDices, int Children, int Place);

    public record PlayerRole(string Id, string Name, string? ImageId, string Description, 
        PlayerRoleDetails Details, 
        IEnumerable<AssetEntry> Assets, 
        IEnumerable<LiabilityEntry> Liabilities,
        IEnumerable<IncomeFlowEntry> Incomes,
        IEnumerable<ExpenseFlowEntry> Expenses)
    {
        private IEnumerable<FinancialFlowEntry> Flows => new List<FinancialFlowEntry>().Union(Incomes).Union(Expenses);

        public int CashFlow => Flows.Sum(x => x.Value);
        
        public bool IsBankrupt => CashFlow < 0; 
    }

    public record PlayerRoleDetails(int Salary, int InitialSavings, int ExpensesForOneChild);

    public record FinancialEntryInfo(string Id, string Name, int Order, 
        FinancialEntryInfo.FinancialType? EntryType, int? Percent)
    {
        public enum FinancialType 
        {
           Default,
           Countable,
           Percent
        }
    }



    public abstract record FinancialFlowEntry(string Name, int Amount)
    {
        public virtual int Value => Amount;
    }

    public record ExpenseFlowEntry(string Name, int Amount) : FinancialFlowEntry(Name, Amount)
    {
        public override int Value => -Amount;
    }

    public record IncomeFlowEntry(string Name, int Amount) : FinancialFlowEntry(Name, Amount);


    public abstract record FinancialEntry(string FinancialEntryInfoId, string Name, int Amount)
    {
        public virtual int Value => Amount;

        public FinancialFlowEntry? GetFinancialFlowEntryByInfo(FinancialEntryInfo info, PlayerRoleDetails details, PlayerState? playerState)
        {
            return info.EntryType switch
            {
                FinancialEntryInfo.FinancialType.Countable => 
                    InitEntry(Name, -details.ExpensesForOneChild * (playerState?.Children ?? 0)),
                FinancialEntryInfo.FinancialType.Percent => 
                    InitEntry(Name, (int)((info.Percent ?? 0) / 100.0  * Value)),
                _ => null
            };
        }

        private static FinancialFlowEntry InitEntry(string name, int value)
        {
            if (value < 0)
            {
                return new ExpenseFlowEntry(name, -value);
            }

            return new IncomeFlowEntry(name, value);
        }
    }

    public record LiabilityEntry(string FinancialEntryInfoId, string Name, int Amount) 
        : FinancialEntry(FinancialEntryInfoId, Name, Amount)
    {
        public override int Value => -Amount;
    }

    public record AssetEntry(string FinancialEntryInfoId, string Name, int Amount) 
        : FinancialEntry(FinancialEntryInfoId, Name, Amount);

    public record RoomInfo(string Id, string Name, string? Password);
}
