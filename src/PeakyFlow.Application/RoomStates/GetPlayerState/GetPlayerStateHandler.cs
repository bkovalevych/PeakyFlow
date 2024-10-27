using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.RoomStates.GetPlayerState
{
    public class GetPlayerStateHandler(IRepository<RoomState> rep) : IRequestHandler<GetPlayerStateQuery, Result<PlayerStateDto>>
    {
        public async Task<Result<PlayerStateDto>> Handle(GetPlayerStateQuery request, CancellationToken cancellationToken)
        {
            await rep.Init();
            var roomState = await rep.GetByIdAsync(request.RoomId);

            if (roomState == null) 
            {
                return Result<PlayerStateDto>.NotFound();
            }

            var p = roomState.PlayerStates
                .FirstOrDefault(x => x.Id == request.PlayerId);

            if (p == null)
            {
                return Result<PlayerStateDto>.NotFound();
            }

            return new PlayerStateDto(p.Id, p.Name, request.RoomId, p.Savings, p.IsBankrupt,
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
