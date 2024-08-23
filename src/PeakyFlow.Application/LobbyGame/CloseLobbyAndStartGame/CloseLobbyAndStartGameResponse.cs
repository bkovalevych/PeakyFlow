using PeakyFlow.Abstractions;

namespace PeakyFlow.Application.LobbyGame.CloseLobbyAndStartGame
{
    public record CloseLobbyAndStartGameResponse(string LobbyId, string Name, IEnumerable<PlayerBase> Players);
}
