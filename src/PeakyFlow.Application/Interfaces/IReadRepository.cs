using Ardalis.Specification;

namespace PeakyFlow.Application.Interfaces
{
    public interface IReadRepository<TEntity> : IReadRepositoryBase<TEntity>
        where TEntity : class
    {
    }
}
