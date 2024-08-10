using Ardalis.Specification;
using PeakyFlow.Abstractions.GameMapRuleAggregate;

namespace PeakyFlow.Application.GameMapRules.GetMapRulesForRoom
{
    public class GetMapRulesSpecification : SingleResultSpecification<GameMapRule>
    {
        public GetMapRulesSpecification()
        {
            Query.Include(x => x.Steps);
        }
    }
}
