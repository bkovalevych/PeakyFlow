using PeakyFlow.Abstractions;

namespace PeakyFlow.Application.LobbyGame.CloseLobbyAndStartGame
{
    public record CloseLobbyAndStartGameResponse(string LobbyId, IEnumerable<PlayerBase> Players);
}
