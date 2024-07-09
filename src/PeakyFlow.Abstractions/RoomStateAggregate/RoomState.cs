namespace PeakyFlow.Abstractions.RoomStateAggregate
{
    public class RoomState : Entity, IAggregateRoot
    {
        public IEnumerable<PlayerState> PlayerStates { get; set; } = [];
    }
}
