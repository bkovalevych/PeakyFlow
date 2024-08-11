using Ardalis.Result;
using MediatR;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.RoomStates.Repair
{
    public record RepairCommand(string RoomId, string PlayerId, IEnumerable<string> LiabilityNames, IEnumerable<int> Money) 
        : IRequest<Result<PlayerStateDto>>, IPlayerIsTakingTurnRequest;
}
