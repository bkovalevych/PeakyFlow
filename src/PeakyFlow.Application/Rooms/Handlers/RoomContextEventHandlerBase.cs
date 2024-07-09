using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Abstractions.RoomAggregate.Interfaces;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Rooms.Specifications;

namespace PeakyFlow.Application.Rooms.Handlers
{
    public abstract class RoomContextEventHandlerBase<TEvent> : INotificationHandler<TEvent>
        where TEvent : IRoomContextEvent
    {
        protected readonly IRepository<Room> RoomRepository;

        protected readonly ILogger<RoomContextEventHandlerBase<TEvent>> Logger;

        protected RoomContextEventHandlerBase(IRepository<Room> roomRepository, ILogger<RoomContextEventHandlerBase<TEvent>> logger)
        {
            RoomRepository = roomRepository;
            Logger = logger;
        }

        public async Task Handle(TEvent notification, CancellationToken cancellationToken)
        {
            var room = await RoomRepository.FirstOrDefaultAsync(new FirstOrDefaultRoomSpecification(notification.RoomId), cancellationToken);
            
            if (room == null) 
            {
                Logger.LogWarning("Room {id} was not found", notification.RoomId);
                return;
            }

            await Handle(notification, room, cancellationToken);
        }

        protected abstract Task Handle(TEvent notification, Room room, CancellationToken cancellationToken);
    }
}
