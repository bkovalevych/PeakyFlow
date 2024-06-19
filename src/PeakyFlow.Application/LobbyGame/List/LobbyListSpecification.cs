using Ardalis.Specification;
using PeakyFlow.Abstractions.LobbyAggregate;

namespace PeakyFlow.Application.LobbyGame.List
{
    public class LobbyListSpecification : Specification<Lobby>
    {
        public LobbyListSpecification(LobbyListQuery query)
        {
            Query
                .AsNoTracking()
                .OrderBy(x => x.LobbyInfo.Created)
                .Skip(query.PaginationSkip)
                .Take(query.PaginationCount)
                .Include(x => x.LobbyInfo);
        }
    }
}
