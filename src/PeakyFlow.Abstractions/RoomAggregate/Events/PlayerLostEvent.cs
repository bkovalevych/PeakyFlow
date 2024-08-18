using PeakyFlow.Abstractions.RoomAggregate.Interfaces;

namespace PeakyFlow.Abstractions.RoomAggregate.Events
{
    public record PlayerLostEvent(string RoomId, string PlayerId) : IRoomContextEvent;
}
