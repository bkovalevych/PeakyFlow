using MediatR;

namespace PeakyFlow.Abstractions.GameMapAggregate.Events
{
    public record PlayerTookTurnEvent(string RoomId, string PlayerId, StepType StepType) : INotification;
}
