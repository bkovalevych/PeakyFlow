namespace PeakyFlow.Abstractions.RoomAggregate
{
    public class Room : Entity, IAggregateRoot
    {
        public string Name { get; set; } = string.Empty;

        public IEnumerable<PlayerInRoom> Players { get; set; } = new List<PlayerInRoom>();
    }
}
