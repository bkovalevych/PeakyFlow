namespace PeakyFlow.Abstractions.LobbyAggregate
{
    public class LobbyInfo
    {
        public LobbyInfo(string id, string owner, string name, DateTimeOffset created, string? password = null)
        {
            Id = id;
            Owner = owner;
            Name = name;
            Password = password;
            Created = created;
        }

        public string Id { get; private set; }
        public string Owner { get; private set; }
        public string Name { get; private set; }
        public string? Password { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
