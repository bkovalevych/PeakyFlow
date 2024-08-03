namespace PeakyFlow.Infrastructure.Redis.Models
{
    public class GameMapPlayerM : PlayerBaseM
    {
        public int Position { get; set; }
        public int Level { get; set; }
        public int SkeepTurns { get; set; }
    }
}
