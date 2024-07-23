using PeakyFlow.Abstractions.GameMapAggregate.Events;

namespace PeakyFlow.Abstractions.GameMapAggregate
{
    public class GameMap : Entity, IAggregateRoot
    {
        public GameMapPlayer[] GameMapPlayers { get; set; } = [];

        public StepType[] Steps { get; set; } = [];

        public int TakingTurnPlayerIndex { get; set; }

        public async Task TakeTurn(Func<PlayerAskedToThrowDiceEvent, Task<int>> throwDice,
            Func<PlayerTookTurnEvent, Task> takeTurn)
        {
            var player = GameMapPlayers[TakingTurnPlayerIndex];

            TakingTurnPlayerIndex = (TakingTurnPlayerIndex + 1) % GameMapPlayers.Length;

            if (player.SkeepTurns > 0)
            {
                --player.SkeepTurns;
                await takeTurn(new PlayerTookTurnEvent(Id, player.Id, StepType.Downsize));
                return;
            }

            var dice = await throwDice(new PlayerAskedToThrowDiceEvent(Id, player.Id));

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

            await takeTurn(new PlayerTookTurnEvent(Id, player.Id, currentPosition));
        }
    }
}
