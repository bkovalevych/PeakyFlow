using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    public abstract class EntityM
    {
        [RedisIdField]
        [Indexed]
        public required string Id { get; set; }
    }
}
