using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.RoomStates.Borrow
{
    public class BorrowHandler : IRequestHandler<BorrowCommand, Result<PlayerStateDto>>
    {
        private readonly IRepository<RoomState> _roomStateRepository;
        private readonly IMediator _mediator;
        private readonly IGuid _guid;

        public BorrowHandler(IGuid guid, IMediator mediator, IRepository<RoomState> roomStateRepository)
        {
            _roomStateRepository = roomStateRepository;
            _mediator = mediator;
            _guid = guid;
        }

        public async Task<Result<PlayerStateDto>> Handle(BorrowCommand request, CancellationToken cancellationToken)
        {
            var state = await _roomStateRepository
                .GetByIdAsync(request.RoomId, cancellationToken);

            if (state == null)
            {
                return Result<PlayerStateDto>.NotFound();
            }

            var p = state.Borrow(request.PlayerId, request.Money, _guid.NewId());
            await _roomStateRepository.UpdateAsync(state, cancellationToken);

            if (p == null)
            {
                return Result<PlayerStateDto>.NotFound();
            }

            await _mediator.Publish(new AnotherPlayerStateChangedEvent(
                request.RoomId,
                request.PlayerId,
                p.PercentageToWin,
                p.HasWon,
                p.HasLost), cancellationToken);

            return Result.Success(new PlayerStateDto(p.Id, p.Name, state.Id, p.Savings,
                p.IsBankrupt,
                p.CountableLiabilities,
                p.PercentableLiabilities,
                p.Stocks,
                p.FinancialItems,
                p.Salary,
                p.Expenses,
                p.Income,
                p.CashFlow,
                p.PercentageToWin,
                p.HasWon,
                p.HasLost,
                p.ExpensesForOneChild));
        }
    }
}
