using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameMapAggregate;
using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    [Document(StorageType = StorageType.Hash, Prefixes = [nameof(GameMap)])]
    public class GameMapM : EntityM
    {
        public GameMapPlayerM[] GameMapPlayers { get; set; } = [];
        public StepType[] Steps { get; set; } = [];

        public string? TakingTurnPlayer { get; set; }
    }
}
