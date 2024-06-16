using FluentValidation;

namespace PeakyFlow.Application.LobbyGame.Create
{
    public class CreateLobbyValidator : AbstractValidator<CreateLobbyCommand>
    {
        public CreateLobbyValidator()
        {
            RuleFor(x => x.Owner).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.TeamSize).InclusiveBetween(1, 6);
        }
    }
}
