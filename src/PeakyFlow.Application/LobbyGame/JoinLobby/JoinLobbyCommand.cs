using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.LobbyGame.JoinLobby
{
    public record JoinLobbyCommand(string LobbyId, string PlayerName, string? Password) 
        : IRequest<Result<JoinLobbyResponse>>;
}
