namespace PeakyFlow.Abstractions.LobbyAggregate
{
    public class Lobby : Entity, IAggregateRoot
    {
        public string Owner { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Password { get; set; }
        public DateTimeOffset Created { get; set; }

        private readonly List<PlayerInLobby> _players = new();
        
        public int PlayersNumber { get; private set; }

        public bool IsFree => PlayersNumber < TeamSize;

        public bool IsPublic => Password == null;

        public IReadOnlyCollection<PlayerInLobby> Players => _players;
        
        public int TeamSize { get; private set; } = 1;

        public bool IsClosed { get; set; }

        public void SetTeamSize(int teamSize)
        {
            TeamSize = teamSize;
        }

        public void SetPassword(string? password)
        {
            Password = password;
        }

        public bool AddPlayer(PlayerInLobby player) 
        {
            if (_players.Contains(player)) 
            {
                return true;
            }

            _players.Add(player);
            PlayersNumber++;

            return true;
        }

        public bool RemovePlayer(string playerId)
        {
            var player = _players.FirstOrDefault(x => x.Id == playerId);
            
            if (player == null) 
            { 
                return true;
            }

            var result = _players.Remove(player);

            if (result)
            {
                --PlayersNumber;
            }

            return result;
        }
    }
}
