using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Abstractions.RoomAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.Rooms.Handlers
{
    public class PlayerLostHandler : RoomContextEventHandlerBase<PlayerLostEvent>
    {
        public PlayerLostHandler(IRepository<Room> roomRepository, ILogger<RoomContextEventHandlerBase<PlayerLostEvent>> logger) : base(roomRepository, logger)
        {
        }

        protected override async Task Handle(PlayerLostEvent notification, Room room, CancellationToken cancellationToken)
        {
            var player = room.Players.FirstOrDefault(x => x.Id == notification.PlayerId);

            if (player == null) 
            {
                Logger.LogWarning("Player {id} in room {idroom} was not found", notification.PlayerId, notification.RoomId);
                return;
            }

            player.Status = PlayerInRoomStatus.Lost;

            await RoomRepository.UpdateAsync(room, cancellationToken);
        }
    }
}
