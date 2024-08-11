using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate.Interfaces;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.Rooms.Handlers
{
    public abstract class RoomStateContextEventHandlerBase<TEvent> : INotificationHandler<TEvent>
        where TEvent : IRoomStateContextEvent
    {
        protected readonly IRepository<RoomState> RoomRepository;

        protected readonly ILogger<RoomStateContextEventHandlerBase<TEvent>> Logger;

        protected virtual bool ThrowWhenNotFound => false;

        protected RoomStateContextEventHandlerBase(IRepository<RoomState> roomRepository, ILogger<RoomStateContextEventHandlerBase<TEvent>> logger)
        {
            RoomRepository = roomRepository;
            Logger = logger;
        }

        public async Task Handle(TEvent notification, CancellationToken cancellationToken)
        {
            var room = await RoomRepository.FirstOrDefaultAsync(
                new FirstOrDefaultByIdSpecification<RoomState>(notification.RoomId),
                cancellationToken);

            if (room == null)
            {
                Logger.LogWarning("RoomState {id} was not found", notification.RoomId);
                
                if (ThrowWhenNotFound)
                {
                    throw new ArgumentNullException(nameof(RoomState));
                }

                return;
            }

            await Handle(notification, room, cancellationToken);
        }

        protected abstract Task Handle(TEvent notification, RoomState room, CancellationToken cancellationToken);
    }
}
