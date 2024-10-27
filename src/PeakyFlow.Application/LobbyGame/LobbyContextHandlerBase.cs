using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame
{
    public abstract class LobbyContextHandlerBase<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ILobbyContextRequest
    {
        protected readonly ILogger<LobbyContextHandlerBase<TRequest, TResponse>> Logger;
        protected readonly IRepository<Lobby> Repository;
        
        public LobbyContextHandlerBase(
            ILogger<LobbyContextHandlerBase<TRequest, TResponse>> logger,
            IRepository<Lobby> repository)
        {
            Logger = logger;
            Repository = repository;
        }
        
        protected abstract TResponse NotFoundResponse { get; }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Request lobby {lobbyId}", request.LobbyId);
            await Repository.Init();
            var lobby = await Repository.GetByIdAsync(request.LobbyId, cancellationToken);

            if (lobby == null)
            {
                Logger.LogInformation("lobby {lobbyId} was not found", request.LobbyId);
                return NotFoundResponse;
            }

            return await Handle(request, lobby, cancellationToken);
        }

        

        protected abstract Task<TResponse> Handle(TRequest request, Lobby lobby, CancellationToken cancellationToken);
    }
}
