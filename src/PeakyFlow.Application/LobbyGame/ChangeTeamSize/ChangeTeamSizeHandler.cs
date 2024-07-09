using Ardalis.Result;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.ChangeTeamSize
{
    public class ChangeTeamSizeHandler : LobbyContextHandlerBase<ChangeTeamSizeCommand, Result>
    {
        public ChangeTeamSizeHandler(
            ILogger<ChangeTeamSizeHandler> logger,
            IRepository<Lobby> lobbyRepository) : base(logger, lobbyRepository)
        { }

        protected override Result NotFoundResponse => Result.NotFound();

        protected override async Task<Result> Handle(ChangeTeamSizeCommand request, Lobby lobby, CancellationToken cancellationToken)
        {
            lobby.SetTeamSize(request.TeamSize);
            await Repository.UpdateAsync(lobby, cancellationToken);
            await Repository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
