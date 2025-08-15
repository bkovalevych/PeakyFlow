using Ardalis.Result;
using AutoMapper;
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
        IMapper mapper,
        ILogger<CreateLobbyHandler> _logger) 
        : IRequestHandler<CreateLobbyCommand, Result<LobbyDto>>
    {    

        public async Task<Result<LobbyDto>> Handle(CreateLobbyCommand request, CancellationToken cancellationToken)
        {
            await _lobbyRepository.Init();
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
            lobby.AddPlayer(new PlayerInLobby() { Id = ownerId, LobbyId = lobbyId, Name = request.Owner, IsOwner = true, IsReady = true });

            try
            {
                var lobbyResult = await _lobbyRepository.AddAsync(lobby, cancellationToken);

                if (lobbyResult == null) 
                {
                    _logger.LogWarning("Data created lobby {lobby} error", request.Name);
                    return Result<LobbyDto>.Error("Data created error");
                }

                _logger.LogInformation("Created lobby with name {name}", request.Name);

                var createdEvent = new LobbyCreatedEvent(lobby.Id, lobby.Name, lobby.OwnerId, lobby.Password, lobby.Created, lobby.IsPublic, lobby.IsFree, lobby.PlayersNumber, lobby.TeamSize);
                await _mediator.Publish(createdEvent, cancellationToken);
            }
            catch (Exception ex) 
            {
                _logger.LogWarning(ex, "Creeating lobby {lobby} error", request.Name);
                return Result<LobbyDto>.Error("Data created error. Exception was thrown");
            }

            var result = mapper.Map<LobbyDto>(lobby);
            
            return result;
        }
    }
}
