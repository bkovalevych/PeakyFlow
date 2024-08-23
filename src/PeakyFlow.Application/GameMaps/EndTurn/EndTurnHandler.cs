using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.GameMaps.EndTurn
{
    public class EndTurnHandler : IRequestHandler<EndTurnCommand, Result>
    {
        private readonly IRepository<GameMap> _gameMapRepository;
        private readonly IMediator _mediator;

        public EndTurnHandler(IRepository<GameMap> gameMapRepository, IMediator mediator)
        {
            _gameMapRepository = gameMapRepository;
            _mediator = mediator;
        }

        public async Task<Result> Handle(EndTurnCommand request, CancellationToken cancellationToken)
        {
            var gameMap = await _gameMapRepository.FirstOrDefaultAsync(new FirstOrDefaultByIdSpecification<GameMap>(request.RoomId), cancellationToken);

            if (gameMap == null || !gameMap.GameMapPlayers.Any(x => x.Id == request.PlayerId))
            {
                return Result.NotFound();
            }

            var endEvnt = new PlayerEndedTurnEvent(request.RoomId, request.PlayerId);

            await _mediator.Publish(endEvnt, cancellationToken);

            var nextPlayers = gameMap.GameMapPlayers.Where(x =>
                    x.Id == request.PlayerId ||
                    !endEvnt.OfflinePlayers.Contains(x.Id)
                    && !endEvnt.PlayersNotToTakeTurn.Contains(x.Id))
                .ToList();
            
            var currentPlayerIndex = nextPlayers.FindIndex(x => x.Id == request.PlayerId);

            if (currentPlayerIndex == -1) 
            {
                return Result.NotFound();
            }

            var nextIndex = (currentPlayerIndex + 1) % nextPlayers.Count;

            var nextPlayer = nextPlayers[nextIndex];


            gameMap.TakingTurnPlayer = nextPlayer.Id;
            await _gameMapRepository.UpdateAsync(gameMap);

            if (endEvnt.OfflinePlayers.Contains(nextPlayer.Id) || 
                endEvnt.PlayersNotToTakeTurn.Contains(nextPlayer.Id))
            {
                await _mediator.Publish(new GameEndedEvent(request.RoomId), cancellationToken);
            }

            var nextStartEvent = new PlayerStartedTurnEvent(request.RoomId, nextPlayer.Id);
            await _mediator.Publish(nextStartEvent, cancellationToken);

            return Result.Success();
        }
    }
}
