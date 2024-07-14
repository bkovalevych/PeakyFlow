using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    public abstract class PlayerBaseM : EntityM
    {
        [Indexed]
        public required string Name { get; set; }
    }
}
