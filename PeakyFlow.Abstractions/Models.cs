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

    public record CurrenPlayerInRoom(PlayerInRoom PlayerInfo, PlayerRole Role);

    public record PlayerRole(string Id, string Name, string? ImageId, string Description, 
        PlayerRoleDetails Details, IEnumerable<FinancialEntry> Finances);

    public record PlayerRoleDetails(int Salary, int ExpensesForOneChild, int CashFlow, int Expenses);

    public record FinancialEntryInfo(string Name, int Order, 
        FinancialEntryInfo.FinancialType? EntryType)
    {
        public enum FinancialType 
        {
           Default,
           Countable
        }
    }

    public record FinancialEntry(string Name, int Amount)
    {
        public virtual int Value => Amount;
    }

    public record ExpenseEntry(string Name, int Amount) : FinancialEntry(Name, Amount)
    {
        public override int Value => -Amount;
    }

    public record IncomeEntry(string Name, int Amount) : FinancialEntry(Name, Amount);
    
    public record RoomInfo(string Id, string Name, string? Password);
}
