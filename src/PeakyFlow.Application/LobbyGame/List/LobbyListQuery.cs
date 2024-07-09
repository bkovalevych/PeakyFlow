using Ardalis.Result;
using MediatR;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.List
{
    public class LobbyListQuery : IRequest<Result<IEnumerable<LobbyListResponse>>>, ICacheQuery, IPaginationQuery
    {
        public string CacheKey => $"lobbies_take{PaginationCount}_skip{PaginationSkip}";

        public TimeSpan MaxAge => TimeSpan.FromSeconds(10);

        public int PaginationCount { get; set; } = 20;
        public int PaginationSkip { get; set; }
    }
}
