using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.LeaveLobby
{
    public class LeaveLobbyHandler : LobbyContextHandlerBase<LeaveLobbyCommand, Result>
    {
        private readonly IMediator _mediator;

        public LeaveLobbyHandler(ILogger<LeaveLobbyHandler> logger, IRepository<Lobby> repository, IMediator mediator) : base(logger, repository)
        {
            _mediator = mediator;
        }

        protected override Result NotFoundResponse => Result.NotFound();

        protected override async Task<Result> Handle(LeaveLobbyCommand request, Lobby lobby, CancellationToken cancellationToken)
        {
            lobby.RemovePlayer(request.PlayerId);
            await Repository.UpdateAsync(lobby, cancellationToken);
            await Repository.SaveChangesAsync(cancellationToken);

            var playerLeft = new PlayerLeftLobbyEvent(lobby.Id, request.PlayerId);

            await _mediator.Publish(playerLeft, cancellationToken);

            return Result.Success();
        }
    }
}
