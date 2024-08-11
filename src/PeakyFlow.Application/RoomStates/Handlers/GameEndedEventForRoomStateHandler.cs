using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Rooms.Handlers;

namespace PeakyFlow.Application.RoomStates.Handlers
{
    public class GameEndedEventForRoomStateHandler : RoomStateContextEventHandlerBase<GameEndedEvent>
    {
        public GameEndedEventForRoomStateHandler(IRepository<RoomState> roomRepository, ILogger<RoomStateContextEventHandlerBase<GameEndedEvent>> logger) : base(roomRepository, logger)
        {
        }

        protected override async Task Handle(GameEndedEvent notification, RoomState room, CancellationToken cancellationToken)
        {
            await RoomRepository.DeleteAsync(room, cancellationToken);
            await RoomRepository.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("Room state {id} was removed", room.Id);
        }
    }
}
