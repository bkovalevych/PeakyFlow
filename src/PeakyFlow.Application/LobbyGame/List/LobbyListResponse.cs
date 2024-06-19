namespace PeakyFlow.Application.LobbyGame.List
{
    public class LobbyListResponse
    {
        public string? Id { get; set; }

        public string? Owner { get; set; }

        public string? Name { get; set; }

        public int TeamSize { get; set; }

        public bool IsPublic { get; set; }

        public bool IsFree { get; set; }

        public int PlayersNumber { get; set; }

        public DateTimeOffset Created { get; set; }
    }
}
