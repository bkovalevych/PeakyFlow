using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.RoomStates.AcceptCard
{
    public record AcceptCardCommand(
        string RoomId,
        string PlayerId,
        string CardId,
        int? Count,
        IEnumerable<string>? FinancialItemIds,
        IEnumerable<Proposition>? Propositions) : IRequest<Result<AcceptCardResponse>>, IPlayerIsTakingTurnRequest;
}
