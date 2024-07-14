using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    internal class PlayerBaseM : EntityM
    {
        [Indexed]
        public required string Name { get; set; }
    }
}
