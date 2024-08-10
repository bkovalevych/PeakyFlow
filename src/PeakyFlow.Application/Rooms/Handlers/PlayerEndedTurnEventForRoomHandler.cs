using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.Rooms.Handlers
{
    public class PlayerEndedTurnEventForRoomHandler : RoomContextEventHandlerBase<PlayerEndedTurnEvent>
    {
        public PlayerEndedTurnEventForRoomHandler(IRepository<Room> roomRepository, ILogger<RoomContextEventHandlerBase<PlayerEndedTurnEvent>> logger) : base(roomRepository, logger)
        {
        }

        protected override Task Handle(PlayerEndedTurnEvent notification, Room room, CancellationToken cancellationToken)
        {
            notification.OfflinePlayers = room.Players
                .Where(x => x.Status != PlayerInRoomStatus.Active)
                .Select(x => x.Id)
                .ToList();

            return Task.CompletedTask;
        }
    }
}
