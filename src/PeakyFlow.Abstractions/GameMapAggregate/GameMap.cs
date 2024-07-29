using PeakyFlow.Abstractions.GameMapAggregate.Events;

namespace PeakyFlow.Abstractions.GameMapAggregate
{
    public class GameMap : Entity, IAggregateRoot
    {
        public GameMapPlayer[] GameMapPlayers { get; set; } = [];

        public StepType[] Steps { get; set; } = [];

        public StepType? MovePlayer(string playerId, int dice)
        {
            var player = GameMapPlayers.FirstOrDefault(x => x.Id == playerId);

            if (player == null) 
            {
                return null;
            }

            if (player.SkeepTurns > 0)
            {
                --player.SkeepTurns;
                return StepType.Downsize;
            }

            player.Position = (player.Position + dice) % Steps.Length;

            var currentPosition = Steps[player.Position];

            if (currentPosition == StepType.Start)
            {
                player.Position = (player.Position + 1) % Steps.Length;
            }

            if (currentPosition == StepType.Downsize)
            {
                player.SkeepTurns = 3;
            }

            return currentPosition;
        }
    }
}
