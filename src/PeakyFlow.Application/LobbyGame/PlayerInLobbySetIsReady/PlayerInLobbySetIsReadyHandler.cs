using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.LobbyGame.PlayerInLobbySetIsReady
{
    public class PlayerInLobbySetIsReadyHandler
        (IRepository<Lobby> _lobbyRepository,
        ILogger<PlayerInLobbySetIsReadyHandler> _logger,
        IMediator _mediator)
        : IRequestHandler<PlayerInLobbySetIsReadyCommand, Result>
    {
        public async Task<Result> Handle(PlayerInLobbySetIsReadyCommand request, CancellationToken cancellationToken)
        {
            var lobby = await _lobbyRepository.FirstOrDefaultAsync(
                new FirstOrDefaultByIdSpecification<Lobby>(request.LobbyId),
                cancellationToken);

            if (lobby == null) 
            {
                _logger.LogInformation("Lobby {idLobby} was not found", request.LobbyId);
                return Result.NotFound();
            }

            var player = lobby.Players.FirstOrDefault(x => x.Id == request.PlayerId);

            if (player == null) 
            {
                _logger.LogInformation("Player {idPlayer} was not found", request.PlayerId);
                return Result.NotFound();
            }

            player.IsReady = request.IsReady;
            await _lobbyRepository.SaveChangesAsync(cancellationToken);
            var playersIsReadyEvent = new PlayerInLobbyIsReadyEvent(request.LobbyId, request.PlayerId, request.IsReady);

            await _mediator.Publish(playersIsReadyEvent, cancellationToken);
            
            return Result.Success();
        }
    }
}
