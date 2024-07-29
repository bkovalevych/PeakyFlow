using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions;

namespace PeakyFlow.Application.RoomStates.PullDealCard
{
    public record PullDealCardCommand(string RoomId, string PlayerId, CardType CardType) 
        : IRequest<Result<Card>>;
}
