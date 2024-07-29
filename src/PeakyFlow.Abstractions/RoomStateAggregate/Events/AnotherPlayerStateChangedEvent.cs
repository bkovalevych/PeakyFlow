using MediatR;

namespace PeakyFlow.Abstractions.RoomStateAggregate.Events
{
    public record AnotherPlayerStateChangedEvent(string RoomStateId, string PlayerId, float PercentageToWin) : INotification;
}
