namespace PeakyFlow.Abstractions.LobbyAggregate
{
    public class Lobby : IAggregateRoot
    {
        private readonly List<PlayerInLobby> _players = new();

        public Lobby(LobbyInfo lobbyInfo)
        {
            LobbyInfo = lobbyInfo;
        }

        public int PlayersNumber { get; private set; }

        public bool IsFree => PlayersNumber < TeamSize;

        public bool IsPublic => LobbyInfo.Password == null;

        public IReadOnlyCollection<PlayerInLobby> Players => _players;
        
        public LobbyInfo LobbyInfo { get; private set; }

        public int TeamSize { get; private set; } = 1;

        public void SetTeamSize(int teamSize)
        {
            TeamSize = teamSize;
        }

        public void SetPassword(string? password)
        {
            LobbyInfo.Password = password;
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

        public bool RemovePlayer(PlayerInLobby player)
        {
            var result = _players.Remove(player);

            if (result)
            {
                --PlayersNumber;
            }

            return result;
        }
    }
}
