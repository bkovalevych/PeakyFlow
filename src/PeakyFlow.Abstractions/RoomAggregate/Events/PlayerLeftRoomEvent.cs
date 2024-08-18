using MediatR;
using PeakyFlow.Abstractions.RoomAggregate.Interfaces;

namespace PeakyFlow.Abstractions.RoomAggregate.Events
{
    public record PlayerLeftRoomEvent(string RoomId, string PlayerId) : IRoomContextEvent;
}
