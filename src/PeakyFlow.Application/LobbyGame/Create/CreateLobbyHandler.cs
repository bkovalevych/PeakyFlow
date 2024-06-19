using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.Create
{
    public class CreateLobbyHandler(
        IMediator _mediator,
        IRepository<Lobby> _lobbyRepository, 
        IGuid _guid,
        IDateProvider _date,
        ILogger<CreateLobbyHandler> _logger) 
        : IRequestHandler<CreateLobbyCommand, Result<string?>>
    {    

        public async Task<Result<string?>> Handle(CreateLobbyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start creating lobby with name {name}", request.Name);
            var id = _guid.NewId();

            var lobbyInfo = new LobbyInfo(id, request.Owner, request.Name, _date.Now, request.Password);
            var lobby = new Lobby(lobbyInfo);

            lobby.SetTeamSize(request.TeamSize);

            try
            {
                var lobbyResult = await _lobbyRepository.AddAsync(lobby, cancellationToken);

                if (lobbyResult == null) 
                {
                    _logger.LogWarning("Data created lobby {lobby} error", request.Name);
                    return Result<string?>.Error("Data created error");
                }

                _logger.LogInformation("Created lobby with name {name}", request.Name);

                var createdEvent = new LobbyCreatedEvent(lobbyInfo);
                await _mediator.Publish(createdEvent, cancellationToken);
            }
            catch (Exception ex) 
            {
                _logger.LogWarning(ex, "Creeating lobby {lobby} error", request.Name);
                return Result<string?>.Error("Data created error. Exception was thrown");
            }

            return id;
        }
    }
}
