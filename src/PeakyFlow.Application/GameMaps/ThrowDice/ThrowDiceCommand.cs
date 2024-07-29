using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.GameMaps.ThrowDice
{
    public record ThrowDiceCommand(string RoomId, string PlayerId, int Dice) : IRequest<Result<ThrowDiceResponse>>;
}
