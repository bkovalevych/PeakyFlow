using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.LobbyGame.LeaveLobby
{
    public record LeaveLobbyCommand(string LobbyId, string PlayerId): IRequest<Result>, ILobbyContextRequest;
}
