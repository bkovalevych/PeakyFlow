using PeakyFlow.Abstractions;

namespace PeakyFlow.Infrastructure.SpreadSheets
{
    public interface ISheetsRetriever<TEntity>
        where TEntity : Entity, IAggregateRoot
    {
        string Range { get; }

        List<TEntity> Retrieve(IList<IList<object>> objects);
    }
}
