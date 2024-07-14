using PeakyFlow.Abstractions.RoomAggregate;
using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = [nameof(Room)])]
    internal class RoomM : EntityM
    {
        [Indexed]
        public string Name { get; set; } = string.Empty;

        [Indexed]
        public IEnumerable<PlayerInRoomM> Players { get; set; } = new List<PlayerInRoomM>();
    }
}
