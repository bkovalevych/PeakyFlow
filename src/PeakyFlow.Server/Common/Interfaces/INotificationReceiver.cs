using MediatR;

namespace PeakyFlow.Server.Common.Interfaces
{
    public interface INotificationReceiver<TNotification>
        where TNotification : INotification
    {
        Task Push(TNotification notification, CancellationToken ct);
        IObservable<TNotification> ReceiveNotifications();
    }
}
