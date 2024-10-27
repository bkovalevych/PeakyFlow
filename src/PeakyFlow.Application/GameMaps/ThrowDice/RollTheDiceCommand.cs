using Ardalis.Result;
using MediatR;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.GameMaps.RollTheDice
{
    public record RollTheDiceCommand(string RoomId, string PlayerId, int Dice) : IRequest<Result<RollTheDiceResponse>>, IPlayerIsTakingTurnRequest;
}
