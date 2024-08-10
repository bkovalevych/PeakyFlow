using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameMapAggregate;
using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = [nameof(GameMap)])]
    public class GameMapM : EntityM
    {
        [Indexed(JsonPath = "$.Id")]
        public GameMapPlayerM[] GameMapPlayers { get; set; } = [];
        public StepType[] Steps { get; set; } = [];
    }
}
