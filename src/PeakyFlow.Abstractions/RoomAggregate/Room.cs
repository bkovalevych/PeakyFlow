namespace PeakyFlow.Abstractions.RoomAggregate
{
    public class Room : Entity, IAggregateRoot
    {
        public string Name { get; set; } = string.Empty;

        public IEnumerable<PlayerInRoom> Players { get; set; } = new List<PlayerInRoom>();

        public PlayerInRoom? SetStatus(string playerId, PlayerInRoomStatus roomStatus)
        {
            return WithExistingPlayer(playerId, player =>
            {
                player.Status = roomStatus;
            });
        }

        private PlayerInRoom? WithExistingPlayer(string playerId, Action<PlayerInRoom> action)
        {
            var player = Players.FirstOrDefault(p => p.Id == playerId);

            if (player != null) 
            {
                action(player);
            }

            return player;
        }
    }
}
