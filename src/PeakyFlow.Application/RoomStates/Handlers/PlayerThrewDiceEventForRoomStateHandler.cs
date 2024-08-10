using MediatR;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameCardRuleAggregate;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.RoomStates.Handlers
{
    public class PlayerThrewDiceEventForRoomStateHandler : INotificationHandler<PlayerThrewDiceEvent>
    {
        private readonly IReadRepository<GameCardRule> _cardRepository;
        private readonly IRepository<RoomState> _roomStateRepository;

        public PlayerThrewDiceEventForRoomStateHandler(
            IRepository<RoomState> roomSateRepository,
            IReadRepository<GameCardRule> cardRepository)
        {
            _cardRepository = cardRepository;
            _roomStateRepository = roomSateRepository;
        }

        public async Task Handle(PlayerThrewDiceEvent notification, CancellationToken cancellationToken)
        {
            var cardRule = await _cardRepository.FirstOrDefaultAsync(new FirstOrDefaultCardRuleSpecification(), cancellationToken);

            if (cardRule == null)
            {
                throw new ArgumentNullException(nameof(cardRule));
            }

            var room = await _roomStateRepository.FirstOrDefaultAsync(new FirstOrDefaultByIdSpecification<RoomState>(notification.RoomId), cancellationToken);

            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }

            var cardId = string.Empty;

            switch (notification.StepType)
            {
                case StepType.Market:
                    cardId = room.GetCardIdByType(CardType.Market);
                    break;
                case StepType.MoneyToTheWind:
                    cardId = room.GetCardIdByType(CardType.MoneyToTheWind);
                    break;
            }

            notification.Card = cardRule.Cards.FirstOrDefault(x => x.Id == cardId);

            await _roomStateRepository.UpdateAsync(room, cancellationToken);
            await _roomStateRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
