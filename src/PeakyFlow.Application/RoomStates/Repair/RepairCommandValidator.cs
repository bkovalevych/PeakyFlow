using FluentValidation;

namespace PeakyFlow.Application.RoomStates.Repair
{
    public class RepairCommandValidator : AbstractValidator<RepairCommand>
    {
        public RepairCommandValidator()
        {
            RuleFor(x => x.RoomStateId).NotEmpty();

            RuleFor(x => x.PlayerId).NotEmpty();

            RuleFor(x => x.Money)
                .NotEmpty();

            RuleFor(x => x.LiabilityNames)
                .NotEmpty()
                .Must((command, names) => command.Money.Count() == command.LiabilityNames.Count());
        }
    }
}
