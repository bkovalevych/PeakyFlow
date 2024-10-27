using Ardalis.Specification;

namespace PeakyFlow.Application.Common.Interfaces
{
    public interface IRepository<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class
    {
        Task Init();
    }
}
