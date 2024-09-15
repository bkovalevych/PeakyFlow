using MediatR;

namespace PeakyFlow.Abstractions.LobbyAggregate.Events
{
    public record LobbyClosedAndGameStartedEvent(string LobbyId, string  Name, 
        string? TakingTurnPlayerId,
        IEnumerable<PlayerBase> Players) : INotification;
}
