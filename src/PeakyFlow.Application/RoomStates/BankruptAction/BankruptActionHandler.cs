using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.RoomStates.BankruptAction
{
    public class BankruptActionHandler : IRequestHandler<BankruptActionCommand, Result<PlayerStateDto>>
    {
        private readonly IRepository<RoomState> _roomStateRep;

        public BankruptActionHandler(IRepository<RoomState> roomStateRep)
        {
            _roomStateRep = roomStateRep;
        }

        public async Task<Result<PlayerStateDto>> Handle(BankruptActionCommand request, CancellationToken cancellationToken)
        {
            var room = await _roomStateRep.GetByIdAsync(request.RoomId);
            
            if (room == null)
            {
                return Result<PlayerStateDto>.NotFound();
            }

            var p = room.BankruptAction(request.PlayerId, request.AsssetIdsToSell, request.StocksToSell);

            if (p == null) 
            {
                return Result<PlayerStateDto>.NotFound();
            }

            await _roomStateRep.UpdateAsync(room, cancellationToken);

            return new PlayerStateDto(p.Id, p.Name, request.RoomId,
                p.Savings,
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
                p.ExpensesForOneChild);
        }
    }
}
