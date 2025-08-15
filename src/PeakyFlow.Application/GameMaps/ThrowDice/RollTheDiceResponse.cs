using PeakyFlow.Abstractions;
using PeakyFlow.Application.RoomStates;

namespace PeakyFlow.Application.GameMaps.RollTheDice
{
    public record RollTheDiceResponse(string RoomId, string PlayerId, StepType StepType, Card? Card, PlayerStateDto? PlayerState);
}
