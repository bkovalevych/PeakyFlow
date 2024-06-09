using MediatR;

namespace PeakyFlow.Abstractions.LobbyAggregate.Events
{
    public record LobbyCreatedEvent(LobbyInfo LobbyInfo) : INotification;
    
}
