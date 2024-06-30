using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.LobbyGame.JoinLobby
{
    public class JoinLobbyHandler(ILogger<JoinLobbyHandler> _logger, 
        IRepository<Lobby> _lobbyRepository,
        IGuid _guid) 
        : IRequestHandler<JoinLobbyCommand, Result<JoinLobbyResponse>>
    {
        public async Task<Result<JoinLobbyResponse>> Handle(JoinLobbyCommand request, CancellationToken cancellationToken)
        {
            var lobby = await _lobbyRepository.FirstOrDefaultAsync(
                new FirstOrDefaultByIdSpecification<Lobby>(request.LobbyId), 
                cancellationToken);
            
            if (lobby == null) 
            {
                return Result<JoinLobbyResponse>.NotFound();
            }

            var player = new PlayerInLobby()
            {
                Id = _guid.NewId(),
                LobbyId = request.LobbyId,
                Name = request.PlayerName
            };

            
            lobby.AddPlayer(player);

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);

            var count = await _lobbyRepository.SaveChangesAsync(cancellationToken);
            
            return new JoinLobbyResponse(count > 0, "Success");
        }
    }
}
