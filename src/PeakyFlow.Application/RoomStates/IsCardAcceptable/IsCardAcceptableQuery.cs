using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.RoomStates.IsCardAcceptable
{
    public record IsCardAcceptableQuery(string RoomId, string PlayerId, string CardId, int? Count, IEnumerable<Proposition>? Propositions) : IRequest<Result<IsCardAcceptableResponse>>, IPlayerIsTakingTurnRequest;
}
