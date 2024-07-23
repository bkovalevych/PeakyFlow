namespace PeakyFlow.Abstractions.GameMapAggregate
{
    public class GameMapPlayer : PlayerBase
    {
        public int Position { get; set; }
        public int Level { get; set; }

        public int SkeepTurns { get; set; }
    }
}
