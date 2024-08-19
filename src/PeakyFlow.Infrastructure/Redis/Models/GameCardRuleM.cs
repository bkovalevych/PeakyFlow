using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameCardRuleAggregate;
using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = [nameof(GameCardRule)])]
    public class GameCardRuleM : EntityM
    {
        [Indexed(JsonPath = "$.Id")]
        public Card[] Cards { get; set; } = [];
    }
}
