using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.RoomStates.GetPlayerState
{
    public record GetPlayerStateQuery(string RoomId, string PlayerId) : IRequest<Result<PlayerStateDto>>;
}
