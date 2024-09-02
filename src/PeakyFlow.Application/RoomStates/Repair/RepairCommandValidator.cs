using FluentValidation;

namespace PeakyFlow.Application.RoomStates.Repair
{
    public class RepairCommandValidator : AbstractValidator<RepairCommand>
    {
        public RepairCommandValidator()
        {
            RuleFor(x => x.RoomId).NotEmpty();

            RuleFor(x => x.PlayerId).NotEmpty();

            RuleFor(x => x.Money)
                .NotEmpty();

            RuleFor(x => x.LiabilityNames)
                .NotEmpty()
                .Must((command, names) => command.Money.Count() == command.LiabilityNames.Count())
                .WithMessage("LiabilityNames count must be equal to Money count");
        }
    }
}
