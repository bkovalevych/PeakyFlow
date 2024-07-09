using Ardalis.Specification;
using PeakyFlow.Abstractions.GameRoleAggregate;

namespace PeakyFlow.Application.Roles.GetRoleForPlayer
{
    public class GetRoleForPlayerSpecification : SingleResultSpecification<GameRole>
    {
        public GetRoleForPlayerSpecification(int index)
        {
            Query.Skip(index)
                .Take(1)
                .Include(x => x.CountableLiabilities)
                .Include(x => x.PercentableLiabilities)
                .Include(x => x.Stocks)
                .Include(x => x.FinancialItems);
        }
    }
}
