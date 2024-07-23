using PeakyFlow.Abstractions.GameMapRuleAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.GameMapRules.GetMapRulesForRoom
{
    public class GetMapRulesForRoomService : IGetMapRulesForRoomService
    {
        private IRepository<GameMapRule> _repository;

        public GetMapRulesForRoomService(IRepository<GameMapRule> repository)
        {
            _repository = repository;
        }

        public Task<GameMapRule?> Get(CancellationToken ct)
        {
            return _repository.FirstOrDefaultAsync(new GetMapRulesSpecification(), ct);
        }
    }
}
