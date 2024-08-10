using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Rooms.Handlers;

namespace PeakyFlow.Application.RoomStates.Handlers
{
    internal class PlayerEndedTurnEventForRoomStateHandler : RoomStateContextEventHandlerBase<PlayerEndedTurnEvent>
    {
        public PlayerEndedTurnEventForRoomStateHandler(IRepository<RoomState> roomRepository, ILogger<RoomStateContextEventHandlerBase<PlayerEndedTurnEvent>> logger) : base(roomRepository, logger)
        {
        }

        protected override Task Handle(PlayerEndedTurnEvent notification, RoomState room, CancellationToken cancellationToken)
        {
            notification.PlayersNotToTakeTurn = room.PlayerStates
                .Where(x => x.HasWon || x.HasLost)
                .Select(x => x.Id)
                .ToList();

            return Task.CompletedTask;
        }
    }
}
