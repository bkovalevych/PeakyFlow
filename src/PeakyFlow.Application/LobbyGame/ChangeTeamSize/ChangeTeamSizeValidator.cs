using FluentValidation;

namespace PeakyFlow.Application.LobbyGame.ChangeTeamSize
{
    public class ChangeTeamSizeValidator : AbstractValidator<ChangeTeamSizeCommand>
    {
        public ChangeTeamSizeValidator()
        {
            RuleFor(x => x.TeamSize).NotEqual(0);
        }
    }
}
