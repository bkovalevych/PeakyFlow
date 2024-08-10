using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.RoomStates.Repair
{
    public record RepairCommand(string RoomStateId, string PlayerId, IEnumerable<string> LiabilityNames, IEnumerable<int> Money) : IRequest<Result<PlayerStateDto>>;
}
