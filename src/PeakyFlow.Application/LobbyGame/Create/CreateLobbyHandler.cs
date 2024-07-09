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
            var lobbyId = _guid.NewId();
            var ownerId = _guid.NewId();

            var lobby = new Lobby()
            {
                Id = lobbyId,
                Name = request.Name,
                Created = _date.Now,
                OwnerId = ownerId,
                Owner = request.Owner,
                Password = request.Password,
            };

            lobby.SetTeamSize(request.TeamSize);
            lobby.AddPlayer(new PlayerInLobby() { Id = ownerId, LobbyId = lobbyId, Name = request.Owner, IsOwner = true });

            try
            {
                var lobbyResult = await _lobbyRepository.AddAsync(lobby, cancellationToken);

                if (lobbyResult == null) 
                {
                    _logger.LogWarning("Data created lobby {lobby} error", request.Name);
                    return Result<string?>.Error("Data created error");
                }

                _logger.LogInformation("Created lobby with name {name}", request.Name);

                var createdEvent = new LobbyCreatedEvent(lobby.Id, lobby.Name, lobby.OwnerId, lobby.Password, lobby.Created, lobby.IsPublic, lobby.TeamSize);
                await _mediator.Publish(createdEvent, cancellationToken);
            }
            catch (Exception ex) 
            {
                _logger.LogWarning(ex, "Creeating lobby {lobby} error", request.Name);
                return Result<string?>.Error("Data created error. Exception was thrown");
            }

            return lobbyId;
        }
    }
}
