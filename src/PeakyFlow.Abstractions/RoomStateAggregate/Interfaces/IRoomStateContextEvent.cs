using MediatR;

namespace PeakyFlow.Abstractions.RoomStateAggregate.Interfaces
{
    public interface IRoomStateContextEvent : INotification
    {
        string RoomId { get; }
    }
}
