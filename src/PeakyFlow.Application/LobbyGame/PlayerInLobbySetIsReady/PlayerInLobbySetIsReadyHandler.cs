using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.PlayerInLobbySetIsReady
{
    public class PlayerInLobbySetIsReadyHandler

        : BaseLobbyContextHandler<PlayerInLobbySetIsReadyCommand, Result>
    {
        private readonly IMediator _mediator;

        public PlayerInLobbySetIsReadyHandler(
            ILogger<PlayerInLobbySetIsReadyHandler> logger,
            IRepository<Lobby> repository,
            IMediator mediator
            ) : base(logger, repository)
        {
            _mediator = mediator;
        }

        protected override Result NotFoundResponse => Result.NotFound();

        protected override async Task<Result> Handle(PlayerInLobbySetIsReadyCommand request, Lobby lobby, CancellationToken cancellationToken)
        {
            var player = lobby.Players.FirstOrDefault(x => x.Id == request.PlayerId);

            if (player == null)
            {
                Logger.LogInformation("Player {idPlayer} was not found", request.PlayerId);
                return Result.NotFound();
            }

            player.IsReady = request.IsReady;
            await Repository.SaveChangesAsync(cancellationToken);
            var playersIsReadyEvent = new PlayerInLobbyIsReadyEvent(request.LobbyId, request.PlayerId, request.IsReady);

            await _mediator.Publish(playersIsReadyEvent, cancellationToken);

            return Result.Success();
        }
    }
}
