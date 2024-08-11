using Ardalis.Result;
using MediatR;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.GameMaps.EndTurn
{
    public record EndTurnCommand(string RoomId, string PlayerId) : IRequest<Result>, IPlayerIsTakingTurnRequest;
}
