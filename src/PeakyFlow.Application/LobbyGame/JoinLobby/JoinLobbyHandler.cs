using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.JoinLobby
{
    public class JoinLobbyHandler
        : LobbyContextHandlerBase<JoinLobbyCommand, Result<JoinLobbyResponse>>
    {
        private IGuid _guid;
        private readonly IMediator _mediator;

        public JoinLobbyHandler(
            ILogger<JoinLobbyHandler> logger,
            IRepository<Lobby> lobbyRepository,
            IGuid guid,
            IMediator mediator) : base(logger, lobbyRepository)
        {
            _guid = guid;
            _mediator = mediator;
        }

        protected override Result<JoinLobbyResponse> NotFoundResponse => Result<JoinLobbyResponse>.NotFound();

        protected override async Task<Result<JoinLobbyResponse>> Handle(JoinLobbyCommand request, Lobby lobby, CancellationToken cancellationToken)
        {
            if (!lobby.IsFree)
            {
                Logger.LogInformation("The lobby {lobbyId} is full", lobby.Id);
                return Result<JoinLobbyResponse>.Conflict("The lobby is full.");
            }

            var player = new PlayerInLobby()
            {
                Id = _guid.NewId(),
                LobbyId = request.LobbyId,
                Name = request.PlayerName
            };


            lobby.AddPlayer(player);

            await Repository.UpdateAsync(lobby, cancellationToken);

            var count = await Repository.SaveChangesAsync(cancellationToken);

            var playerJoined = new PlayerJoinedEvent(lobby.Id, player.Id, player.Name);

            await _mediator.Publish(playerJoined, cancellationToken);

            return new JoinLobbyResponse(count > 0, "Success", player.Id);
        }
    }
}
