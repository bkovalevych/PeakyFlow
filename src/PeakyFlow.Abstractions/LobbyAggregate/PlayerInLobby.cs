namespace PeakyFlow.Abstractions.LobbyAggregate
{
    public class PlayerInLobby : PlayerBase
    {
        public required string LobbyId { get; set; }
        public bool IsReady { get; set; }
    }
}
