namespace PeakyFlow.Abstractions.GameMapRuleAggregate
{
    public class GameMapRule : Entity, IAggregateRoot
    {
        public StepType[] Steps { get; set; } = [];
    }
}
