using PeakyFlow.Abstractions;
using PeakyFlow.Application.RoomStates;

namespace PeakyFlow.Application.GameMaps.ThrowDice
{
    public record ThrowDiceResponse(string RoomId, string PlayerId, StepType StepType, Card? Card, PlayerStateDto? PlayerState);
}
