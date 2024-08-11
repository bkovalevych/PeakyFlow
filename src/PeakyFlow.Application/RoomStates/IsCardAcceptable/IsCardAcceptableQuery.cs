using Ardalis.Result;
using MediatR;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.RoomStates.IsCardAcceptable
{
    public record IsCardAcceptableQuery(string RoomId, string PlayerId, string CardId, int? Count) : IRequest<Result<IsCardAcceptableResponse>>, IPlayerIsTakingTurnRequest;
}
