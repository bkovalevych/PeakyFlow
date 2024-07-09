using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.LobbyGame.CloseLobbyAndStartGame
{
    public record CloseLobbyAndStartGameCommand(string LobbyId) : IRequest<Result<CloseLobbyAndStartGameResponse>>, ILobbyContextRequest;
}
