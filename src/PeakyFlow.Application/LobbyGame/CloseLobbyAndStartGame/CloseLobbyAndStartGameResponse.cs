using PeakyFlow.Abstractions.LobbyAggregate;

namespace PeakyFlow.Application.LobbyGame.CloseLobbyAndStartGame
{
    public record CloseLobbyAndStartGameResponse(string LobbyId, string Name, IEnumerable<PlayerInLobby> Players);
}
