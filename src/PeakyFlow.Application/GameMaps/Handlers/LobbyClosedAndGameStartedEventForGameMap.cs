using MediatR;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.GameMapRules.GetMapRulesForRoom;

namespace PeakyFlow.Application.GameMaps.Handlers
{
    public class LobbyClosedAndGameStartedEventForGameMap : INotificationHandler<LobbyClosedAndGameStartedEvent>
    {
        private readonly IGetMapRulesForRoomService _getMapRulesForRoomService;
        private readonly IRepository<GameMap> _gameMapRepository;
        private readonly IMediator _mediator;

        public LobbyClosedAndGameStartedEventForGameMap(
            IRepository<GameMap> repository,
            IGetMapRulesForRoomService getMapRulesForRoomService,
            IMediator mediator)
        {
            _getMapRulesForRoomService = getMapRulesForRoomService;
            _gameMapRepository = repository;
            _mediator = mediator;
        }

        public async Task Handle(LobbyClosedAndGameStartedEvent notification, CancellationToken cancellationToken)
        {
            var steps = await _getMapRulesForRoomService.Get(cancellationToken);

            if (steps == null)
            {
                throw new ArgumentException("Rules were not defined");
            }

            var map = new GameMap()
            {
                Id = notification.LobbyId,
                Steps = steps.Steps
            };

            var players = new List<GameMapPlayer>();

            foreach (var item in notification.players)
            {
                var player = new GameMapPlayer()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Level = 0,
                    Position = 0,
                    SkeepTurns = 0
                };

                players.Add(player);
            }

            map.GameMapPlayers = players.ToArray();

            await _gameMapRepository.AddAsync(map, cancellationToken);
        }
    }
}
