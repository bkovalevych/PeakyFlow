using FluentValidation;

namespace PeakyFlow.Application.RoomStates.Borrow
{
    public class BorrowCommandValidator : AbstractValidator<BorrowCommand>
    {
        public BorrowCommandValidator()
        {
            RuleFor(x => x.Money)
                .GreaterThan(0);

            RuleFor(x => x.PlayerId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.RoomId)
                .NotNull()
                .NotEmpty();
        }
    }
}
