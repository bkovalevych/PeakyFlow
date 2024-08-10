using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.GameCardRuleAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.RoomStates.AcceptCard
{
    public class AcceptCardHandler : IRequestHandler<AcceptCardCommand, Result<AcceptCardResponse>>
    {
        private readonly IRepository<RoomState> _roomStateRepository;
        private readonly IReadRepository<GameCardRule> _gameCardRuleRepository;

        public AcceptCardHandler(IRepository<RoomState> roomStateRepository,
            IReadRepository<GameCardRule> gameCardRuleRepository)
        {
            _roomStateRepository = roomStateRepository;
            _gameCardRuleRepository = gameCardRuleRepository;
        }

        public async Task<Result<AcceptCardResponse>> Handle(AcceptCardCommand request, CancellationToken cancellationToken)
        {
            var cardRule = await _gameCardRuleRepository.FirstOrDefaultAsync(new FirstOrDefaultCardRuleSpecification(), cancellationToken);

            if (cardRule == null)
            {
                return Result<AcceptCardResponse>.NotFound();
            }

            var room = await _roomStateRepository.FirstOrDefaultAsync(new FirstOrDefaultByIdSpecification<RoomState>(request.RoomId), cancellationToken);

            if (room == null)
            {
                return Result<AcceptCardResponse>.NotFound();
            }

            var card = cardRule.Cards.First(x => x.Id == request.CardId);

            var p = room.AcceptCard(card, request.PlayerId, request.Count, request.financialItemIds);

            
            if (p.PlayerState == null)
            {
                return Result<AcceptCardResponse>.NotFound();
            }

            await _roomStateRepository.SaveChangesAsync(cancellationToken);

            var playerState = new PlayerStateDto(p.PlayerState.Id, p.PlayerState.Name,
                room.Id, p.PlayerState.Savings, p.PlayerState.IsBankrupt,
                p.PlayerState.CountableLiabilities, p.PlayerState.PercentableLiabilities,
                p.PlayerState.Stocks, p.PlayerState.FinancialItems, p.PlayerState.Salary,
                p.PlayerState.Expenses, p.PlayerState.Income, p.PlayerState.CashFlow, p.PlayerState.HasWon);

            return new AcceptCardResponse(playerState, p.Acceptable);
        }
    }
}
