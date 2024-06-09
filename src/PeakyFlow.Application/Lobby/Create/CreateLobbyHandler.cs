using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PeakyFlow.Application.Lobby.Create
{
    public class CreateLobbyHandler(ILogger<CreateLobbyHandler> _logger) 
        : IRequestHandler<CreateLobbyCommand, Result<string>>
    {
        

        public Task<Result<string>> Handle(CreateLobbyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start creating lobby with name {name}", request.Name);

            throw new NotImplementedException();
        }
    }
}
