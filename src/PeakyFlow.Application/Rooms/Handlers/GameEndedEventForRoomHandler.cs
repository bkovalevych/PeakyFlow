using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.Rooms.Handlers
{
    public class GameEndedEventForRoomHandler : RoomContextEventHandlerBase<GameEndedEvent>
    {
        public GameEndedEventForRoomHandler(IRepository<Room> roomRepository, ILogger<RoomContextEventHandlerBase<GameEndedEvent>> logger) : base(roomRepository, logger)
        {
        }

        protected override async Task Handle(GameEndedEvent notification, Room room, CancellationToken cancellationToken)
        {
            await RoomRepository.DeleteAsync(room, cancellationToken);
            await RoomRepository.SaveChangesAsync(cancellationToken);
            Logger.LogInformation("Room {id} was removed", room.Id);
        }
    }
}
