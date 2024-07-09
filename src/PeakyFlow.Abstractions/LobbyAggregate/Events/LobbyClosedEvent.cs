using MediatR;

namespace PeakyFlow.Abstractions.LobbyAggregate.Events
{
    public record LobbyClosedEvent(string LobbyId) : INotification;
}
