namespace PeakyFlow.Abstractions.LobbyAggregate
{
    public record Lobby(LobbyInfo LobbyInfo, IEnumerable<PlayerInLobby> Players);
}
