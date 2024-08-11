using MediatR;
using PeakyFlow.Abstractions.RoomAggregate.Interfaces;
using PeakyFlow.Abstractions.RoomStateAggregate.Interfaces;

namespace PeakyFlow.Abstractions.GameMapAggregate.Events
{
    public record GameEndedEvent(string RoomId) : INotification, IRoomStateContextEvent, IRoomContextEvent;
}
