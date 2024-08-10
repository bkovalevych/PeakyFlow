using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.RoomStateAggregate;
using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = [nameof(RoomState)])]
    public class RoomStateM : EntityM
    {
        public Dictionary<CardType, int> Indeces { get; set; } = [];

        public Dictionary<CardType, List<string>> Cards { get; set; } = [];

        [Indexed(JsonPath = "$.Id")]
        public PlayerStateM[] PlayerStates { get; set; } = [];
    }
}
