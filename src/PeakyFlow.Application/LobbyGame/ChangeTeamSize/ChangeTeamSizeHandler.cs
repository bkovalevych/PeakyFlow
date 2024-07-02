using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.LobbyGame.ChangeTeamSize
{
    public class ChangeTeamSizeHandler(ILogger<ChangeTeamSizeHandler> _logger, IRepository<Lobby> _lobbyRepository) : IRequestHandler<ChangeTeamSizeCommand, Result>
    {
        public async Task<Result> Handle(ChangeTeamSizeCommand request, CancellationToken cancellationToken)
        {
            var lobby = await _lobbyRepository.FirstOrDefaultAsync(
                new FirstOrDefaultByIdSpecification<Lobby>(request.LobbyId),
                cancellationToken);

            if (lobby == null)
            {
                _logger.LogInformation("Lobby {idLobby} was not found", request.LobbyId);
                return Result.NotFound();
            }

            lobby.SetTeamSize(request.TeamSize);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);
            
            return Result.Success();
        }
    }
}
