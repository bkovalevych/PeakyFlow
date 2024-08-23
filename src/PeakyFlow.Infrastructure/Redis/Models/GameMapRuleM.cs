using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameRoleAggregate;
using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = [nameof(GameRole)])]
    public class GameMapRuleM : EntityM
    {
        public StepType[] Steps { get; set; } = [];
    }
}
