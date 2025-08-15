using Azure;
using PeakyFlow.Abstractions.RoomAggregate;
using System.Text.Json;

namespace PeakyFlow.Infrastructure.AzureTable.Models
{
    internal class RoomTableEntity : AzureMappingEntity<Room>
    {
        public string Name { get; set; } = string.Empty;

        public string PlayersRaw { get; set; } = string.Empty;

        public override Room ParseFromTableEntity()
        {
            var players = JsonSerializer.Deserialize<List<PlayerInRoom>>(PlayersRaw ?? string.Empty)
                ?? [];

            return new Room() { Id = RowKey, Name = Name, Players = players, ETag = ETag.ToString() };
        }

        public override AzureMappingEntity<Room> SerializeToTableEntity(Room entity, string partitionKey)
        {
            PartitionKey = partitionKey;
            RowKey = entity.Id;
            Name = entity.Name;
            ETag = string.IsNullOrEmpty(entity.ETag)
                ? ETag.All
                : new ETag(entity.ETag);
            PlayersRaw = JsonSerializer.Serialize(entity.Players);

            return this;
        }
    }
}
