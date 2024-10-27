using Azure;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.RoomStateAggregate;
using System.Text.Json;

namespace PeakyFlow.Infrastructure.AzureTable.Models
{
    internal class RoomStateTableEntity : AzureMappingEntity<RoomState>
    {
        public string IndecesRaw { get; set; } = string.Empty;

        public string CardsRaw { get; set; } = string.Empty;

        public string PlayerStatesRaw { get; set; } = string.Empty;


        public override RoomState ParseFromTableEntity()
        {
            var indeces = JsonSerializer.Deserialize<Dictionary<CardType, int>>(IndecesRaw) 
                ?? [];

            var cards = JsonSerializer.Deserialize<Dictionary<CardType, List<string>>>(CardsRaw)
                ?? [];
            
            var playerStates = JsonSerializer.Deserialize<List<PlayerState>>(PlayerStatesRaw)
                ?? [];
            
            var state = new RoomState() 
            { 
                Id = RowKey,
                Indeces = indeces,
                Cards = cards,
                PlayerStates = playerStates,
                ETag = ETag.ToString()
            };

            return state;
        }

        public override AzureMappingEntity<RoomState> SerializeToTableEntity(RoomState entity, string partitionKey)
        {
            PartitionKey = partitionKey;
            RowKey = entity.Id;

            IndecesRaw = JsonSerializer.Serialize(entity.Indeces);
            CardsRaw = JsonSerializer.Serialize(entity.Cards);
            PlayerStatesRaw = JsonSerializer.Serialize(entity.PlayerStates);
            ETag = string.IsNullOrEmpty(entity.ETag)
                ? ETag.All
                : new ETag(entity.ETag);
            return this;
        }
    }
}
