using Ardalis.Specification;

namespace PeakyFlow.Application.Common.Interfaces
{
    public interface IReadRepository<TEntity> : IReadRepositoryBase<TEntity>
        where TEntity : class
    {
    }
}
