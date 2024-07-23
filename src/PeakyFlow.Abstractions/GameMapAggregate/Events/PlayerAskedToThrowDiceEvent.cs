using MediatR;

namespace PeakyFlow.Abstractions.GameMapAggregate.Events
{
    public record PlayerAskedToThrowDiceEvent(string MapId, string PlayerId) : INotification
    {
         public int Dice { get; set; }
    }
}
