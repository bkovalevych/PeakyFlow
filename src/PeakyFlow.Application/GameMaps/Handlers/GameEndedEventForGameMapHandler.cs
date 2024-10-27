using MediatR;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.GameMaps.Handlers
{
    public class GameEndedEventForGameMapHandler : INotificationHandler<GameEndedEvent>
    {
        private readonly IRepository<GameMap> _rep;

        public GameEndedEventForGameMapHandler(IRepository<GameMap> rep)
        {
            _rep = rep;
        }

        public async Task Handle(GameEndedEvent notification, CancellationToken cancellationToken)
        {
            await _rep.Init();
            var gameMap = await _rep.GetByIdAsync(notification.RoomId, cancellationToken);
            if (gameMap == null) 
            {
                return;
            }

            await _rep.DeleteAsync(gameMap, cancellationToken);
        }
    }
}
