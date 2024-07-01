using MediatR;

namespace PeakyFlow.Abstractions.LobbyAggregate.Events
{
    public record PlayerInLobbyIsReadyEvent(string LobbyId, string PlayerId, bool IsReady) : INotification;
}
