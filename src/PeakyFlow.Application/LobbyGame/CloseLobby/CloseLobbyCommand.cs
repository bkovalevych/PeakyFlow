using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.LobbyGame.CloseLobby
{
    public record CloseLobbyCommand(string LobbyId, string PlayerId) : IRequest<Result>, ILobbyContextRequest;
}
