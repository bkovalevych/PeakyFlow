using PeakyFlow.Abstractions.LobbyAggregate;
using Redis.OM.Modeling;

namespace PeakyFlow.Infrastructure.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = [nameof(Lobby)])]
    internal class LobbyM : EntityM
    {
        public string Owner { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Password { get; set; }
        public DateTimeOffset Created { get; set; }

        public int PlayersNumber { get; set; }

        public bool IsFree { get; set; }

        public bool IsPublic { get; set; }

        [Indexed(JsonPath = "$.Id")]
        public PlayerInLobby[] Players { get; set; } = [];

        public int TeamSize { get; set; } = 1;

        public bool IsClosed { get; set; }
    }
}
