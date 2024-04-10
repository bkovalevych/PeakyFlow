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

    

    public abstract record FinancialFlowEntry(string Name, int Amount, string? FinancialEntryId = null)
    {
        public virtual int Value => Amount;
    }

    public record ExpenseFlowEntry(string Name, int Amount, string? FinancialEntryId = null) : FinancialFlowEntry(Name, Amount, FinancialEntryId)
    {
        public override int Value => -Amount;
    }

    public record IncomeFlowEntry(string Name, int Amount, string? FinancialEntryId = null) : FinancialFlowEntry(Name, Amount, FinancialEntryId);


    public abstract record FinancialEntry(string Id, string FinancialEntryInfoId, string Name, int Amount)
    {
        public virtual int Value => Amount;

        public FinancialFlowEntry? GetFinancialFlowEntryByInfo(FinancialEntryInfo info, PlayerRoleDetails details, PlayerState? playerState)
        {
            return info.EntryType switch
            {
                FinancialEntryInfo.FinancialType.Countable => 
                    InitEntry(Name, -details.ExpensesForOneChild * (playerState?.Children ?? 0), Id),
                FinancialEntryInfo.FinancialType.Percent => 
                    InitEntry(Name, (int)((info.Percent ?? 0) / 100.0  * Value), Id),
                _ => null
            };
        }

        private static FinancialFlowEntry InitEntry(string name, int value, string? financialEntryId = null)
        {
            if (value < 0)
            {
                return new ExpenseFlowEntry(name, -value, financialEntryId);
            }

            return new IncomeFlowEntry(name, value, financialEntryId);
        }
    }

    public record LiabilityEntry(string Id, string FinancialEntryInfoId, string Name, int Amount) 
        : FinancialEntry(Id, FinancialEntryInfoId, Name, Amount)
    {
        public override int Value => -Amount;
    }

    public record AssetEntry(string Id, string FinancialEntryInfoId, string Name, int Amount, AssetEntry.AssetType AssetEntryType, int? Count, int? Price) 
        : FinancialEntry(Id, FinancialEntryInfoId, Name, Amount)
    {
        public enum AssetType
        {
            Default,
            Savings,
            Stock,
            Business,
            RealEstate
        }
    }
    
    public record RoomInfo(string Id, string Name, string? Password);
}
