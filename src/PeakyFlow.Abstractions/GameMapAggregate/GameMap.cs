namespace PeakyFlow.Abstractions.GameMapAggregate
{
    public class GameMap : Entity, IAggregateRoot
    {
        public GameMapPlayer[] GameMapPlayers { get; set; } = [];

        public StepType[] Steps { get; set; } = [];

        public string? TakingTurnPlayer { get; set; }

        public (StepType? StepType, bool WithSalary) MovePlayer(string playerId, int dice)
        {
            var player = GameMapPlayers.FirstOrDefault(x => x.Id == playerId);

            if (player == null)
            {
                return (null, false);
            }

            if (player.SkeepTurns > 0)
            {
                --player.SkeepTurns;
                return (StepType.DownsizeWait, false);
            }

            var oldPosition = player.Position;

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

            var withSalary = WasSalaryStepRiched(player.Position, oldPosition);

            return (currentPosition, withSalary);
        }

        private bool WasSalaryStepRiched(int currentPosition, int oldPosition)
        {
            if (Steps[currentPosition] == StepType.Salary)
            {
                return true;
            }

            for (var i = (oldPosition + 1) % Steps.Length; 
                i != currentPosition; i = (i + 1) % Steps.Length)
            {
                if (Steps[i] == StepType.Salary)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
