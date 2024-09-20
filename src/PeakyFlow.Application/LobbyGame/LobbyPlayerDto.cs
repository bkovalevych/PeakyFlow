namespace PeakyFlow.Application.LobbyGame
{
    public record LobbyPlayerDto(string Id, string Name, string LobbyId, bool IsReady, bool IsOwner);
}
