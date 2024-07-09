using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Abstractions.RoomAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.Rooms.Handlers
{
    public class PlayerLeftHandler : RoomContextEventHandlerBase<PlayerLeftEvent>
    {
        public PlayerLeftHandler(IRepository<Room> roomRepository, ILogger<RoomContextEventHandlerBase<PlayerLeftEvent>> logger) : base(roomRepository, logger)
        {
        }

        protected override async Task Handle(PlayerLeftEvent notification, Room room, CancellationToken cancellationToken)
        {
            var player = room.Players.FirstOrDefault(x => x.Id == notification.PlayerId);

            if (player == null)
            {
                Logger.LogWarning("Player {id} in room {idroom} was not found. When he left", notification.PlayerId, notification.RoomId);
                return;
            }

            player.Status = PlayerInRoomStatus.Out;

            await RoomRepository.UpdateAsync(room, cancellationToken);
            await RoomRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
