using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.RoomStates.Borrow
{
    public record BorrowCommand(string RoomStateId, string PlayerId, int Money) : IRequest<Result<PlayerStateDto>>;
}
