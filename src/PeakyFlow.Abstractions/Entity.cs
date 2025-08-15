namespace PeakyFlow.Abstractions
{
    public abstract class Entity
    {
        public required string Id { get; set; }

        public string? ETag { get; set; }
    }
}
