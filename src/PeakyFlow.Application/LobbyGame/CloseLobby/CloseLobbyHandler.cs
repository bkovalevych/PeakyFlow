using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.CloseLobby
{
    public class CloseLobbyHandler : LobbyContextHandlerBase<CloseLobbyCommand, Result>
    {
        private readonly IMediator _mediator;

        public CloseLobbyHandler(IMediator mediator, ILogger<CloseLobbyHandler> logger, IRepository<Lobby> repository) : base(logger, repository)
        {
            _mediator = mediator;
        }

        protected override Result NotFoundResponse => Result.NotFound();

        protected override async Task<Result> Handle(CloseLobbyCommand request, Lobby lobby, CancellationToken cancellationToken)
        {
            if (lobby.OwnerId != request.PlayerId)
            {
                Logger.LogInformation("Player {playerId} tried to close lobby {lobbyName}, who is not owner", request.PlayerId, lobby.Name);
                return Result.Conflict();
            }

            await Repository.DeleteAsync(lobby, cancellationToken);
            var closedEvent = new LobbyClosedEvent(request.LobbyId);

            await _mediator.Publish(closedEvent, cancellationToken);

            return Result.Success();
        }
    }
}
