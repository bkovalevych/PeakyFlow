namespace PeakyFlow.Infrastructure.Redis.Models
{
    internal class PlayerInLobbyM : PlayerBaseM
    {
        public required string LobbyId { get; set; }
        public bool IsReady { get; set; }
        public bool IsOwner { get; set; }
    }
}
