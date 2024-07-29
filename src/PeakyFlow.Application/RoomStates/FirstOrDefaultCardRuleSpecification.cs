using Ardalis.Specification;
using PeakyFlow.Abstractions.GameCardRuleAggregate;

namespace PeakyFlow.Application.RoomStates
{
    public class FirstOrDefaultCardRuleSpecification : SingleResultSpecification<GameCardRule>
    {
        public FirstOrDefaultCardRuleSpecification()
        {
            Query.Include(x => x.Cards);
        }
    }
}
