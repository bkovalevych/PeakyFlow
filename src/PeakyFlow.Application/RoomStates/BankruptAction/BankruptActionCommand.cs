using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions;

namespace PeakyFlow.Application.RoomStates.BankruptAction
{
    public record BankruptActionCommand(
        string RoomId,
        string PlayerId,
        IEnumerable<string>? AsssetIdsToSell,
        IEnumerable<Proposition>? StocksToSell
        ) : IRequest<Result<PlayerStateDto>>;
}
