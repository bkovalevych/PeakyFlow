using Ardalis.Result;
using MediatR;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.RoomStates.Borrow
{
    public record BorrowCommand(string RoomId, string PlayerId, int Money) : IRequest<Result<PlayerStateDto>>, IPlayerIsTakingTurnRequest;
}
