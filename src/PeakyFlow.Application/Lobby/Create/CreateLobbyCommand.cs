using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.Lobby.Create
{
    public record CreateLobbyCommand(string Owner, string Name, string? Password) 
        : IRequest<Result<string>>;
}
