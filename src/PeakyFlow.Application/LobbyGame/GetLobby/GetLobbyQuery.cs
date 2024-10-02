using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.LobbyGame.GetLobby
{
    public record GetLobbyQuery(string LobbyId, string PlayerId) : ILobbyContextRequest, IRequest<Result<LobbyDto>>;
}
