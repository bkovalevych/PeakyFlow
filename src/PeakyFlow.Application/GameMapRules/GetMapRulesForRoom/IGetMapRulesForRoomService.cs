using PeakyFlow.Abstractions.GameMapRuleAggregate;

namespace PeakyFlow.Application.GameMapRules.GetMapRulesForRoom
{
    public interface IGetMapRulesForRoomService
    {
        Task<GameMapRule?> Get(CancellationToken ct);
    }
}
