using PeakyFlow.Abstractions.RoomAggregate;
using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    internal class PlayerInRoomM : PlayerBaseM
    {
        [Indexed]
        public PlayerInRoomStatus Status { get; set; }
    }
}
