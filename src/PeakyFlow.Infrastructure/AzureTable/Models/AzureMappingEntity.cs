using PeakyFlow.Abstractions;

namespace PeakyFlow.Infrastructure.AzureTable.Models
{
    internal abstract class AzureMappingEntity<TEntity> : AzureTableEntity
        where TEntity : IAggregateRoot
    {
        public abstract TEntity ParseFromTableEntity();
        public abstract AzureMappingEntity<TEntity> SerializeToTableEntity(TEntity entity, string partitionKey);
    }
}
