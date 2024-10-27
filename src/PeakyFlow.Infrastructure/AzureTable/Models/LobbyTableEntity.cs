using Azure;
using PeakyFlow.Abstractions.LobbyAggregate;
using System.Text.Json;

namespace PeakyFlow.Infrastructure.AzureTable.Models
{
    internal class LobbyTableEntity : AzureMappingEntity<Lobby>
    {
        public string Owner { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Password { get; set; }
        
        public DateTimeOffset Created { get; set; }

        public int PlayersNumber { get; set; }

        public string? PlayersSerialized { get; set; }

        public int TeamSize { get; set; } = 1;

        public bool IsClosed { get; set; }


        public override Lobby ParseFromTableEntity()
        {
            var players = JsonSerializer.Deserialize<List<PlayerInLobby>>(PlayersSerialized ?? string.Empty) 
                ?? [];

            var lobby = new Lobby()
            {
                Id = RowKey,
                Owner = Owner,
                Created = Created,
                IsClosed = IsClosed,
                Name = Name,
                Password = Password,
                OwnerId = OwnerId,
                ETag = ETag.ToString()
            };

            lobby.SetTeamSize(TeamSize);

            foreach (var p in players) 
            {
                lobby.AddPlayer(p);
            }

            return lobby;
        }

        public override AzureMappingEntity<Lobby> SerializeToTableEntity(Lobby entity, string partitionKey)
        {
            RowKey = entity.Id;
            PartitionKey = partitionKey;
            Owner = entity.Owner;
            OwnerId = entity.OwnerId;
            Name = entity.Name;
            Password = entity.Password;
            Created = entity.Created;
            PlayersNumber = entity.PlayersNumber;
            PlayersSerialized = JsonSerializer.Serialize(entity.Players);
            TeamSize = entity.TeamSize;
            IsClosed = entity.IsClosed;
            ETag = string.IsNullOrEmpty(entity.ETag)
                ? ETag.All
                : new ETag(entity.ETag);

            return this;
        }
    }
}
