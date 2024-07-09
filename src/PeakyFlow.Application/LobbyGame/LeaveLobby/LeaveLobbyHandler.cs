using Ardalis.Result;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.LeaveLobby
{
    public class LeaveLobbyHandler : LobbyContextHandlerBase<LeaveLobbyCommand, Result>
    {
        public LeaveLobbyHandler(ILogger<LeaveLobbyHandler> logger, IRepository<Lobby> repository) : base(logger, repository)
        {
        }

        protected override Result NotFoundResponse => Result.NotFound();

        protected override async Task<Result> Handle(LeaveLobbyCommand request, Lobby lobby, CancellationToken cancellationToken)
        {
            lobby.RemovePlayer(request.PlayerId);
            await Repository.UpdateAsync(lobby, cancellationToken);
            await Repository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
