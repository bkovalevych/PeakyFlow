using MediatR;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

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
            var gameMap = await _rep.FirstOrDefaultAsync(new FirstOrDefaultByIdSpecification<GameMap>(notification.RoomId), cancellationToken);
            if (gameMap == null) 
            {
                return;
            }

            await _rep.DeleteAsync(gameMap, cancellationToken);
        }
    }
}
