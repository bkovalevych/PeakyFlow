using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.RoomStates.Repair
{
    public class RepairHandler : IRequestHandler<RepairCommand, Result<PlayerStateDto>>
    {
        private readonly IMediator _mediator;
        private readonly IRepository<RoomState> _roomStateRepository;

        public RepairHandler(IMediator mediator, IRepository<RoomState> roomStateRepository)
        {
            _mediator = mediator;
            _roomStateRepository = roomStateRepository;
        }

        public async Task<Result<PlayerStateDto>> Handle(RepairCommand request, CancellationToken cancellationToken)
        {
            var state = await _roomStateRepository
                .FirstOrDefaultAsync(new FirstOrDefaultByIdSpecification<RoomState>(request.RoomId), cancellationToken);

            if (state == null)
            {
                return Result<PlayerStateDto>.NotFound();
            }

            var p = state.Repair(request.PlayerId, request.LiabilityNames, request.Money);

            if (p == null)
            {
                return Result<PlayerStateDto>.NotFound();
            }

            await _roomStateRepository.UpdateAsync(state, cancellationToken);

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
                p.HasLost, p.ExpensesForOneChild));
        }
    }
}
