using PeakyFlow.Abstractions.RoomStateAggregate.Interfaces;

namespace PeakyFlow.Abstractions.GameMapAggregate.Events
{
    public record PlayerStartedTurnEvent(string RoomId, string PlayerId) : IRoomStateContextEvent;
}
