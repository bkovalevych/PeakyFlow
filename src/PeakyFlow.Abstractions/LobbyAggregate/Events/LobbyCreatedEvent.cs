using MediatR;

namespace PeakyFlow.Abstractions.LobbyAggregate.Events
{
    public record LobbyCreatedEvent(string Id, string Name, string Owner, string? Password, DateTimeOffset Created, bool IsPublic, int TeamSize) : INotification;
}
