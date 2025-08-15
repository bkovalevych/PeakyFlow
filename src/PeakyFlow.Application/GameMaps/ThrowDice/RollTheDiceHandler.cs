using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.RoomStates;

namespace PeakyFlow.Application.GameMaps.RollTheDice
{
    public class RollTheDiceHandler : IRequestHandler<RollTheDiceCommand, Result<RollTheDiceResponse>>
    {
        private readonly IMediator _mediator;
        private readonly IRepository<GameMap> _gameMapRepository;

        public RollTheDiceHandler(IMediator mediator, IRepository<GameMap> gameMapRepository)
        {
            _mediator = mediator;
            _gameMapRepository = gameMapRepository;
        }

        public async Task<Result<RollTheDiceResponse>> Handle(RollTheDiceCommand request, CancellationToken cancellationToken)
        {
            await _gameMapRepository.Init();
            var gameMap = await _gameMapRepository.GetByIdAsync(request.RoomId, cancellationToken);

            if (gameMap == null)
            {
                return Result<RollTheDiceResponse>.NotFound();
            }

            var (step, withSalary) = gameMap.MovePlayer(request.PlayerId, request.Dice);

            if (step == null)
            {
                return Result<RollTheDiceResponse>.NotFound();
            }

            await _gameMapRepository.UpdateAsync(gameMap);
            
            var playerThrewDiceEvent = new PlayerThrewDiceEvent(request.RoomId, request.PlayerId, step.Value, withSalary);

            await _mediator.Publish(playerThrewDiceEvent, cancellationToken);

            if (playerThrewDiceEvent.PlayerState == null)
            {
                return Result<RollTheDiceResponse>.NotFound();
            }

            var pst = playerThrewDiceEvent.PlayerState;

            return new RollTheDiceResponse(
                request.RoomId,
                request.PlayerId,
                step.Value,
                playerThrewDiceEvent.Card,
                new PlayerStateDto(request.PlayerId,
                    pst.Name,
                    request.RoomId,
                    pst.Savings,
                    pst.IsBankrupt,
                    pst.CountableLiabilities,
                    pst.PercentableLiabilities,
                    pst.Stocks,
                    pst.FinancialItems,
                    pst.Salary,
                    pst.Expenses,
                    pst.Income,
                    pst.CashFlow,
                    pst.PercentageToWin,
                    pst.HasWon,
                    pst.HasLost,
                    pst.ExpensesForOneChild));
        }
    }
}
