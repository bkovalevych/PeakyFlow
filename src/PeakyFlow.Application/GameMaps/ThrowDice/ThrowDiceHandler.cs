using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.GameMaps.ThrowDice
{
    public class ThrowDiceHandler : IRequestHandler<ThrowDiceCommand, Result<ThrowDiceResponse>>
    {
        private readonly IMediator _mediator;
        private readonly IRepository<GameMap> _gameMapRepository;

        public ThrowDiceHandler(IMediator mediator, IRepository<GameMap> gameMapRepository)
        {
            _mediator = mediator;
            _gameMapRepository = gameMapRepository;
        }

        public async Task<Result<ThrowDiceResponse>> Handle(ThrowDiceCommand request, CancellationToken cancellationToken)
        {
            var gameMap = await _gameMapRepository.FirstOrDefaultAsync(new FirstOrDefaultByIdSpecification<GameMap>(request.RoomId), cancellationToken);

            if (gameMap == null)
            {
                return Result<ThrowDiceResponse>.NotFound();
            }

            var step = gameMap.MovePlayer(request.PlayerId, request.Dice);

            if (step == null)
            {
                return Result<ThrowDiceResponse>.NotFound();
            }

            await _gameMapRepository.SaveChangesAsync(cancellationToken);

            var playerThrewDiceEvent = new PlayerThrewDiceEvent(request.RoomId, request.PlayerId, step.Value);

            await _mediator.Publish(playerThrewDiceEvent, cancellationToken);

            return new ThrowDiceResponse(
                request.RoomId,
                request.PlayerId,
                step.Value,
                playerThrewDiceEvent.Card);
        }
    }
}
