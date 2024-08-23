using MediatR;

namespace PeakyFlow.Abstractions.LobbyAggregate.Events
{
    public record PlayerJoinedEvent(string LobbyId, string PlayerId, string PlayerName) : INotification;
}
