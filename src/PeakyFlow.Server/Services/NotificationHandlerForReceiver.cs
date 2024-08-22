using MediatR;
using PeakyFlow.Server.Common.Interfaces;

namespace PeakyFlow.Server.Services
{
    public class NotificationHandlerForReceiver<TNotification>(INotificationReceiver<TNotification> receiver) : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        public async Task Handle(TNotification notification, CancellationToken cancellationToken)
        {
            await receiver.Push(notification, cancellationToken);
        }
    }
}
