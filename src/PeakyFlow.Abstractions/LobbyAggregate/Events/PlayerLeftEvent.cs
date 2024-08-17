using MediatR;

namespace PeakyFlow.Abstractions.LobbyAggregate.Events
{
    public record PlayerLeftEvent(string LobbyId, string PlayerId, string PlayerName) : INotification;
}
