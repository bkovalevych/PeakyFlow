using PeakyFlow.Abstractions;

namespace PeakyFlow.Application.GameMaps.ThrowDice
{
    public record ThrowDiceResponse(string RoomId, string PlayerId, StepType StepType, Card? Card);
}
