using PeakyFlow.Abstractions.RoomAggregate;
using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    [Document(StorageType = StorageType.Hash, Prefixes = [nameof(Room)])]
    public class RoomM : EntityM
    {
        [Indexed]
        public string Name { get; set; } = string.Empty;

        public PlayerInRoomM[] Players { get; set; } = [];
    }
}
