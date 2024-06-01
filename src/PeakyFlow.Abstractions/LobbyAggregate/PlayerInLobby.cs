namespace PeakyFlow.Abstractions.LobbyAggregate
{
    public record PlayerInLobby(string Id, string Name,
       string LobbyId, bool IsReady)
       : PlayerBase(Id, Name);
}
