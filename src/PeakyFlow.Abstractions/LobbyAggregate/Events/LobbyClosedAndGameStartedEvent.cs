using MediatR;

namespace PeakyFlow.Abstractions.LobbyAggregate.Events
{
    public record LobbyClosedAndGameStartedEvent(string LobbyId, string  Name, IEnumerable<PlayerBase> players) : INotification;
}
