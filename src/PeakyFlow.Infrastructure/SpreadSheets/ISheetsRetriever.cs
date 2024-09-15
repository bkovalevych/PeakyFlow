using PeakyFlow.Abstractions;

namespace PeakyFlow.Infrastructure.SpreadSheets
{
    public interface ISheetsRetriever<TEntity>
        where TEntity : Entity, IAggregateRoot
    {
        List<string> Ranges { get; }

        List<TEntity> Retrieve(IList<IList<IList<object>>> ranges);
    }
}
