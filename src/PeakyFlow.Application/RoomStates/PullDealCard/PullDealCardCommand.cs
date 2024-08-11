using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.RoomStates.PullDealCard
{
    public record PullDealCardCommand(string RoomId, string PlayerId, CardType CardType) 
        : IRequest<Result<Card>>, IPlayerIsTakingTurnRequest;
}
