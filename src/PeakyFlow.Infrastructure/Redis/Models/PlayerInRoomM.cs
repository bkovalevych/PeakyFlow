using PeakyFlow.Abstractions.RoomAggregate;
using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    public class PlayerInRoomM
    {
        [Indexed]
        public required string Id { get; set; }

        public required string Name { get; set; }

        [Indexed]
        public PlayerInRoomStatus Status { get; set; }
    }
}
