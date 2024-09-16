using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.LobbyGame.Create
{
    public record CreateLobbyCommand(string Owner, string Name, int TeamSize, string? Password) 
        : IRequest<Result<LobbyDto>>;
}
