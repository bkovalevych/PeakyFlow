using Ardalis.Specification;
using PeakyFlow.Abstractions;

namespace PeakyFlow.Application.Common.Specifications
{
    public class FirstOrDefaultByIdSpecification<TEntity> : SingleResultSpecification<TEntity>
        where TEntity : Entity
    {
        public FirstOrDefaultByIdSpecification(string id)
        {
            Query.Where(x => x.Id == id);
        }
    }
}
