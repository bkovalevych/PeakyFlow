namespace PeakyFlow.Abstractions.LobbyAggregate
{
    public record LobbyInfo(string Id, string Name,
        int NumberOfPlayers, int NumberOfPlaces,
        string? Password, bool IsStarted = false)
    {
        public bool IsPublic => Password == null;
        public bool IsFree => NumberOfPlaces > NumberOfPlayers;
    }
}
