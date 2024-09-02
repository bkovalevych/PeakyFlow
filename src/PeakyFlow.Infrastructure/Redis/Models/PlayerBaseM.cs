using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    public abstract class PlayerBaseM
    {
        [Indexed]
        [RedisIdField]
        public required string Id { get; set; }
        public required string Name { get; set; }
    }
}
