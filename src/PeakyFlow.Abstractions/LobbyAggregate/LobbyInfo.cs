namespace PeakyFlow.Abstractions.LobbyAggregate
{
    public class LobbyInfo
    {
        public LobbyInfo(string id, string name, string? password = null)
        {
            Id = id;
            Name = name;
            Password = password;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string? Password { get; set; }
    }
}
