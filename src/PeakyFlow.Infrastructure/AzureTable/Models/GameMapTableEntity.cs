using Azure;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameMapAggregate;
using System.Text.Json;

namespace PeakyFlow.Infrastructure.AzureTable.Models
{
    internal class GameMapTableEntity : AzureMappingEntity<GameMap>
    {
        public string GameMapPlayersRaw { get; set; } = string.Empty;

        public string StepsRaw { get; set; } = string.Empty;

        public string? TakingTurnPlayer { get; set; }

        public override GameMap ParseFromTableEntity()
        {
            var gameMapPlayers = JsonSerializer.Deserialize<GameMapPlayer[]>(GameMapPlayersRaw)
                ?? [];
            var steps = JsonSerializer.Deserialize<StepType[]>(StepsRaw) ?? [];

            return new GameMap()
            {
                Id = RowKey,
                GameMapPlayers = gameMapPlayers,
                Steps = steps,
                TakingTurnPlayer = TakingTurnPlayer,
                ETag = ETag.ToString()
            };
        }

        public override AzureMappingEntity<GameMap> SerializeToTableEntity(GameMap entity, string partitionKey)
        {
            PartitionKey = partitionKey;
            RowKey = entity.Id;
            TakingTurnPlayer = entity.TakingTurnPlayer;

            GameMapPlayersRaw = JsonSerializer.Serialize(entity.GameMapPlayers);
            StepsRaw = JsonSerializer.Serialize(entity.Steps);
            ETag = string.IsNullOrEmpty(entity.ETag)
                ? ETag.All
                : new ETag(entity.ETag);

            return this;
        }
    }
}
