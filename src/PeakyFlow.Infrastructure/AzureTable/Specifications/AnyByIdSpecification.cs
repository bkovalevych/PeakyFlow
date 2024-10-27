using Ardalis.Specification;
using PeakyFlow.Abstractions;

namespace PeakyFlow.Infrastructure.AzureTable.Specifications
{
    public class AnyByIdSpecification<T> : SingleResultSpecification<T>
        where T : Entity, IAggregateRoot

    {
        public AnyByIdSpecification(string id)
        {
            Query.Where(x => x.Id == id);
        }
    }
}
