using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.RoomStates.Borrow
{
    public class BorrowHandler : IRequestHandler<BorrowCommand, Result<PlayerStateDto>>
    {
        private readonly IRepository<RoomState> _roomStateRepository;
        private readonly IMediator _mediator;

        public BorrowHandler(IMediator mediator, IRepository<RoomState> roomStateRepository)
        {
            _roomStateRepository = roomStateRepository;
            _mediator = mediator;
        }

        public async Task<Result<PlayerStateDto>> Handle(BorrowCommand request, CancellationToken cancellationToken)
        {
            var state = await _roomStateRepository
                .FirstOrDefaultAsync(new FirstOrDefaultByIdSpecification<RoomState>(request.RoomStateId), cancellationToken);

            if (state == null)
            {
                return Result<PlayerStateDto>.NotFound();
            }

            var p = state.Borrow(request.PlayerId, request.Money);

            if (p == null)
            {
                return Result<PlayerStateDto>.NotFound();
            }

            await _mediator.Publish(new AnotherPlayerStateChangedEvent(
                request.RoomStateId, 
                request.PlayerId, 
                p.PercentageToWin), cancellationToken);

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
                p.HasWon));
        }
    }
}
