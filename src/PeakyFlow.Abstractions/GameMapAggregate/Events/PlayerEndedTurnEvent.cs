using PeakyFlow.Abstractions.RoomAggregate.Interfaces;
using PeakyFlow.Abstractions.RoomStateAggregate.Interfaces;

namespace PeakyFlow.Abstractions.GameMapAggregate.Events
{
    public record PlayerEndedTurnEvent(string RoomId, string PlayerId)
        : IRoomContextEvent, IRoomStateContextEvent
    {
        public List<string> PlayersNotToTakeTurn { get; set; } = [];

        public List<string> OfflinePlayers { get; set; } = [];
    }
}
