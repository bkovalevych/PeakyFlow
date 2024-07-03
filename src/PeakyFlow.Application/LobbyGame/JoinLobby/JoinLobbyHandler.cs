using Ardalis.Result;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.JoinLobby
{
    public class JoinLobbyHandler
        : BaseLobbyContextHandler<JoinLobbyCommand, Result<JoinLobbyResponse>>
    {
        private IGuid _guid;

        public JoinLobbyHandler(
            ILogger<JoinLobbyHandler> logger,
            IRepository<Lobby> lobbyRepository,
            IGuid guid) : base(logger, lobbyRepository)
        {
            _guid = guid;
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

            return new JoinLobbyResponse(count > 0, "Success");
        }
    }
}
