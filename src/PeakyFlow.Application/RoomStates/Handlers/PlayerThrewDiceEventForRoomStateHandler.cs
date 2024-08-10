using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameCardRuleAggregate;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Rooms.Handlers;

namespace PeakyFlow.Application.RoomStates.Handlers
{
    public class PlayerThrewDiceEventForRoomStateHandler : RoomStateContextEventHandlerBase<PlayerThrewDiceEvent>
    {
        private readonly IReadRepository<GameCardRule> _cardRepository;

        public PlayerThrewDiceEventForRoomStateHandler(
            IRepository<RoomState> roomSateRepository,
            IReadRepository<GameCardRule> cardRepository,
            ILogger<PlayerThrewDiceEventForRoomStateHandler> logger)
            : base(roomSateRepository, logger)
        {
            _cardRepository = cardRepository;
        }

        protected override bool ThrowWhenNotFound => true;

        protected override async Task Handle(PlayerThrewDiceEvent notification, RoomState room, CancellationToken cancellationToken)
        {
            var cardRule = await _cardRepository.FirstOrDefaultAsync(new FirstOrDefaultCardRuleSpecification(), cancellationToken);

            if (cardRule == null)
            {
                throw new ArgumentNullException(nameof(cardRule));
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

            if (notification.WithSalary)
            {
                room.CountSalary(notification.PlayerId);
            }

            notification.PlayerState = room.PlayerStates
                .FirstOrDefault(x => x.Id == notification.PlayerId);

            await RoomRepository.UpdateAsync(room, cancellationToken);
            await RoomRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
