using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameCardRuleAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.RoomStates.PullDealCard
{
    public class PullDealCardHandler : IRequestHandler<PullDealCardCommand, Result<Card>>
    {
        private readonly IReadRepository<GameCardRule> _cardRepository;
        private readonly IRepository<RoomState> _roomStateRepository;

        public PullDealCardHandler(
           IRepository<RoomState> roomSateRepository,
           IReadRepository<GameCardRule> cardRepository)
        {
            _cardRepository = cardRepository;
            _roomStateRepository = roomSateRepository;
        }

        public async Task<Result<Card>> Handle(PullDealCardCommand request, CancellationToken cancellationToken)
        {
            await _roomStateRepository.Init();
            var cardRule = await _cardRepository.FirstOrDefaultAsync(new FirstOrDefaultCardRuleSpecification(), cancellationToken);

            if (cardRule == null)
            {
                throw new ArgumentNullException(nameof(cardRule));
            }

            var room = await _roomStateRepository.GetByIdAsync(request.RoomId, cancellationToken);

            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }

            var cardId = room.GetCardIdByType(request.CardType);
            await _roomStateRepository.UpdateAsync(room, cancellationToken);
            var card = cardRule.Cards.First(x => x.Id == cardId);
            
            return card;
        }
    }
}
