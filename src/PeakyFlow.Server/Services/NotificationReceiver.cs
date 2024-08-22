using MediatR;
using PeakyFlow.Server.Common.Interfaces;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace PeakyFlow.Server.Services
{
    public class NotificationReceiver<TNotification> : INotificationReceiver<TNotification>
        where TNotification : INotification
    {
        private readonly Subject<TNotification> _subject = new Subject<TNotification>();

        public Task Push(TNotification notification, CancellationToken cancellationToken)
        {
            _subject.OnNext(notification);
            return Task.CompletedTask;
        }

        public IObservable<TNotification> ReceiveNotifications()
        {
            return _subject.AsObservable();
        }
    }
}
