using MediatR;
using PeakyFlow.Abstractions.RoomAggregate.Interfaces;

namespace PeakyFlow.Abstractions.RoomAggregate.Events
{
    public record PlayerLeftEvent(string RoomId, string PlayerId) : IRoomContextEvent;
}
