using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.CloseLobbyAndStartGame
{
    public class CloseLobbyAndStartGameHandler : LobbyContextHandlerBase<CloseLobbyAndStartGameCommand, Result<CloseLobbyAndStartGameResponse>>
    {
        private readonly IMediator _mediator;

        public CloseLobbyAndStartGameHandler(
            IMediator mediator,
            ILogger<CloseLobbyAndStartGameHandler> logger, IRepository<Lobby> repository) : base(logger, repository)
        {
            _mediator = mediator;
        }

        protected override Result<CloseLobbyAndStartGameResponse> NotFoundResponse => Result<CloseLobbyAndStartGameResponse>.NotFound();

        protected override async Task<Result<CloseLobbyAndStartGameResponse>> Handle(CloseLobbyAndStartGameCommand request, Lobby lobby, CancellationToken cancellationToken)
        {
            lobby.IsClosed = true;
            await Repository.UpdateAsync(lobby, cancellationToken);
            await Repository.SaveChangesAsync(cancellationToken);

            var players = lobby.Players
                    .Where(x => x.IsReady);
            var response = new CloseLobbyAndStartGameResponse(
                request.LobbyId,
                players);

            var closedLobby = new LobbyClosedAndGameStartedEvent(request.LobbyId, lobby.Name, players);

            await _mediator.Publish(closedLobby, cancellationToken);

            return response;
        }
    }
}
