using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.RoomStates;

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
            var gameMap = await _gameMapRepository.GetByIdAsync(request.RoomId, cancellationToken);

            if (gameMap == null)
            {
                return Result<ThrowDiceResponse>.NotFound();
            }

            var (step, withSalary) = gameMap.MovePlayer(request.PlayerId, request.Dice);

            if (step == null)
            {
                return Result<ThrowDiceResponse>.NotFound();
            }

            await _gameMapRepository.UpdateAsync(gameMap);
            
            var playerThrewDiceEvent = new PlayerThrewDiceEvent(request.RoomId, request.PlayerId, step.Value, withSalary);

            await _mediator.Publish(playerThrewDiceEvent, cancellationToken);

            if (playerThrewDiceEvent.PlayerState == null)
            {
                return Result<ThrowDiceResponse>.NotFound();
            }

            var pst = playerThrewDiceEvent.PlayerState;

            return new ThrowDiceResponse(
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
