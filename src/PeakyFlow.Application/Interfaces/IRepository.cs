using Ardalis.Specification;

namespace PeakyFlow.Application.Interfaces
{
    public interface IRepository<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class
    {
    }
}
