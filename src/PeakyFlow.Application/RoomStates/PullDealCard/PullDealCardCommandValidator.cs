using FluentValidation;
using PeakyFlow.Abstractions;

namespace PeakyFlow.Application.RoomStates.PullDealCard
{
    public class PullDealCardCommandValidator : AbstractValidator<PullDealCardCommand>
    {
        public PullDealCardCommandValidator()
        {
            RuleFor(x => x.CardType).Must(x => x == CardType.BigDeal || x == CardType.SmallDeal);
        }
    }
}
