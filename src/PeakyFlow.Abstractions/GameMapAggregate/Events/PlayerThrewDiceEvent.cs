using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate.Interfaces;

namespace PeakyFlow.Abstractions.GameMapAggregate.Events
{
    public record PlayerThrewDiceEvent(string RoomId, string PlayerId, StepType StepType, bool WithSalary) : IRoomStateContextEvent
    {
        public Card? Card { get; set; }

        public PlayerState? PlayerState { get; set; }
    }
}
