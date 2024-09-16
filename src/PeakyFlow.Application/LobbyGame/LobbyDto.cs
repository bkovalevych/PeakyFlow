namespace PeakyFlow.Application.LobbyGame
{
    public record LobbyDto(string Id, string Owner, string Name, string? Password, DateTimeOffset Created, int PlayersNumber, bool IsFree, bool IsPublic, int TeamSize, bool IsClosed, IEnumerable<LobbyPlayerDto> Players);
}
