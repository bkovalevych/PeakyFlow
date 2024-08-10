using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.RoomStates.AcceptCard
{
    public record AcceptCardCommand(
        string RoomId,
        string PlayerId,
        string CardId,
        int? Count,
        IEnumerable<string>? financialItemIds) : IRequest<Result<AcceptCardResponse>>;
}
