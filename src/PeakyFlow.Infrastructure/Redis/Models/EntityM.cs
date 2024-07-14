using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    internal abstract class EntityM
    {
        [RedisIdField]
        [Indexed]
        public required string Id { get; set; }
    }
}
