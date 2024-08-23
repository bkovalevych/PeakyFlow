using MediatR;

namespace PeakyFlow.Abstractions.LobbyAggregate.Events
{
    public record PlayerLeftLobbyEvent(string RoomId, string PlayerId) : INotification;
}
