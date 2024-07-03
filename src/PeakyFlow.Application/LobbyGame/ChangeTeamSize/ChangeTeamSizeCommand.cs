using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.LobbyGame.ChangeTeamSize
{
    public record ChangeTeamSizeCommand(string LobbyId, int TeamSize) : IRequest<Result>, ILobbyContextRequest;
}
