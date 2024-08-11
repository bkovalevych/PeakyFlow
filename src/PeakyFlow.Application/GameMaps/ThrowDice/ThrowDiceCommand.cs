using Ardalis.Result;
using MediatR;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.GameMaps.ThrowDice
{
    public record ThrowDiceCommand(string RoomId, string PlayerId, int Dice) : IRequest<Result<ThrowDiceResponse>>, IPlayerIsTakingTurnRequest;
}
