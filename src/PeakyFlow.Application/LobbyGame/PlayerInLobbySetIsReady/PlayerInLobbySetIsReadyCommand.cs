using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.LobbyGame.PlayerInLobbySetIsReady
{
    public record PlayerInLobbySetIsReadyCommand(string LobbyId, string PlayerId, bool IsReady) : IRequest<Result>, ILobbyContextRequest;
}
