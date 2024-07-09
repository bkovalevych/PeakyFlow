using MediatR;

namespace PeakyFlow.Abstractions.RoomAggregate.Interfaces
{
    public interface IRoomContextEvent : INotification
    {
        string RoomId { get; }
    }
}
