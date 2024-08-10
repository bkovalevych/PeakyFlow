using MediatR;

namespace PeakyFlow.Abstractions.GameMapAggregate.Events
{
    public record PlayerThrewDiceEvent(string RoomId, string PlayerId, StepType StepType) : INotification
    {
        public Card? Card { get; set; }
    }
}
