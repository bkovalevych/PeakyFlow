using MediatR;

namespace PeakyFlow.Abstractions.RoomStateAggregate.Events
{
    public record AnotherPlayerStateChangedEvent(string RoomStateId, string PlayerId, float PercentageToWin, bool HasWon, bool HasLost) : INotification;
}
