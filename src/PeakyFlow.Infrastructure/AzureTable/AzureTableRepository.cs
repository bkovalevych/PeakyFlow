using Ardalis.Specification;
using AutoMapper;
using Azure;
using Azure.Data.Tables;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.Common.Exceptions;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Infrastructure.AzureTable.Models;
using System.Linq.Expressions;

namespace PeakyFlow.Infrastructure.AzureTable
{
    internal class AzureTableRepository<TEntity, TMappingEntity> : IRepository<TEntity>
        where TEntity : Entity, IAggregateRoot
        where TMappingEntity : AzureMappingEntity<TEntity>, new()
    {
        private readonly TableServiceClient _client;
        private readonly TableClient _tableClient;
        private readonly IMapper _mapper;
        private const string PartitionKey = "General";

        public AzureTableRepository(TableServiceClient client, IMapper mapper)
        {
            _client = client;
            var tableName = typeof(TEntity).Name;
            _tableClient = _client.GetTableClient(tableName);
            _mapper = mapper;
        }

        public async Task Init()
        {
            await _tableClient.CreateIfNotExistsAsync();
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var m = new TMappingEntity()
                .SerializeToTableEntity(entity, PartitionKey);

            await _tableClient.AddEntityAsync(m, cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var transactions = entities.Select(x =>
                new TableTransactionAction(TableTransactionActionType.UpdateReplace, new TMappingEntity()
                .SerializeToTableEntity(x, PartitionKey)));

            await _tableClient.SubmitTransactionAsync(transactions, cancellationToken);

            return entities;
        }

        public Expression<Func<TMappingEntity, bool>> AddPartitionKeyLambda(Expression<Func<TMappingEntity, bool>> expression, string partitionKey)
        {
            var parameter = expression.Parameters[0];

            var partitioKeyProp = Expression.Property(parameter, "PartitionKey");
            var constantValue = Expression.Constant(partitionKey);
            var condition = Expression.Equal(constantValue, partitioKeyProp);

            var combinedCondition = Expression.AndAlso(condition, expression.Body);

            return Expression.Lambda<Func<TMappingEntity, bool>>(combinedCondition, parameter);
        }

        public async Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var mappedFilter = _mapper.Map<Expression<Func<TMappingEntity, bool>>>(specification.WhereExpressions.FirstOrDefault()?.Filter ?? (x => true));
            var filter = AddPartitionKeyLambda(mappedFilter, PartitionKey);


            var query = _tableClient.QueryAsync(filter, 1, ["PartitionKey", "RowKey"], cancellationToken);

            await foreach (var page in query)
            {
                if (page != null)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            var query = _tableClient.QueryAsync<TMappingEntity>(cancellationToken: cancellationToken);

            await foreach (var item in query)
            {
                if (item != null)
                {
                    return true;
                }
            }

            return false;
        }

        public async IAsyncEnumerable<TEntity> AsAsyncEnumerable(ISpecification<TEntity> specification)
        {
            var mappedFilter = _mapper.Map<Expression<Func<TMappingEntity, bool>>>(specification.WhereExpressions.FirstOrDefault()?.Filter ?? (x => true));
            var filter = AddPartitionKeyLambda(mappedFilter, PartitionKey);

            var query = _tableClient.QueryAsync(filter);

            await foreach (var item in query)
            {
                var original = item.ParseFromTableEntity();
                yield return original;
            }
        }

        public async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var mappedFilter = _mapper.Map<Expression<Func<TMappingEntity, bool>>>(specification.WhereExpressions.FirstOrDefault()?.Filter ?? (x => true));
            var filter = AddPartitionKeyLambda(mappedFilter, PartitionKey);


            var query = _tableClient.QueryAsync(filter: filter, select: ["PartitionKey", "RowKey"], cancellationToken: cancellationToken);
            var count = 0;
            await foreach (var page in query.AsPages())
            {
                if (page != null)
                {
                    count += page.Values.Count;
                }
            }

            return count;
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            Expression<Func<TMappingEntity, bool>> filter = x => x.PartitionKey == PartitionKey;
            
            var query = _tableClient.QueryAsync(filter: filter, select: ["PartitionKey", "RowKey"], cancellationToken: cancellationToken);
            var count = 0;
            await foreach (var page in query.AsPages())
            {
                if (page != null)
                {
                    count += page.Values.Count;
                }
            }

            return count;
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _tableClient.DeleteEntityAsync(PartitionKey, entity.Id, cancellationToken: cancellationToken);
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                await _tableClient.DeleteEntityAsync(PartitionKey, entity.Id, cancellationToken: cancellationToken);
            }
        }

        public async Task DeleteRangeAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            Expression<Func<TMappingEntity, bool>> filter = x => x.PartitionKey == PartitionKey;

            var query = _tableClient.QueryAsync(filter: filter, select: ["PartitionKey", "RowKey"], cancellationToken: cancellationToken);
            var transactions = new List<TableTransactionAction>();

            await foreach (var item in query)
            {
                transactions.Add(new TableTransactionAction(TableTransactionActionType.Delete, item));
            }

            await _tableClient.SubmitTransactionAsync(transactions, cancellationToken);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            Expression<Func<TMappingEntity, bool>> filter = x => x.PartitionKey == PartitionKey;

            var query = _tableClient.QueryAsync(filter: filter, maxPerPage: 1, cancellationToken: cancellationToken);
            
            await foreach (var item in query)
            {
                return item.ParseFromTableEntity();
            }

            return null;
        }

        public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        {
            if (specification.Selector == null)
            {
                return default;
            }

            Expression<Func<TMappingEntity, bool>> filter = x => x.PartitionKey == PartitionKey;

            var query = _tableClient.QueryAsync(filter: filter, maxPerPage: 1, cancellationToken: cancellationToken);

            await foreach (var item in query)
            {
                return specification.Selector.Compile().Invoke(item.ParseFromTableEntity());
            }

            return default;
        }

        public async Task<TEntity?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
        {
            return (await _tableClient.GetEntityIfExistsAsync<TMappingEntity>(PartitionKey, id.ToString(), cancellationToken: cancellationToken))?
                .Value?.ParseFromTableEntity();
        }

        public Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            return FirstOrDefaultAsync(specification, cancellationToken);
        }

        public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        {
            return FirstOrDefaultAsync(specification, cancellationToken);
        }

        public async Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
        {
            Expression<Func<TMappingEntity, bool>> filter = x => x.PartitionKey == PartitionKey;

            var query = _tableClient.QueryAsync(filter);
            
            var list = new List<TEntity>();
            await foreach (var item in query)
            {
                var original = item.ParseFromTableEntity();
                list.Add(original);
            }

            return list;
        }

        public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var list = new List<TEntity>();

            try
            {
                var mappedFilter = _mapper.Map<Expression<Func<TMappingEntity, bool>>>(specification.WhereExpressions.FirstOrDefault()?.Filter ?? (x => true));
                var filter = AddPartitionKeyLambda(mappedFilter, PartitionKey);

                var query = _tableClient.QueryAsync(filter, specification.Take, cancellationToken: cancellationToken);

                var skip = 0;

                await foreach (var page in query.AsPages(pageSizeHint: specification.Take))
                {
                    if (skip != (specification.Skip ?? 0))
                    {
                        skip += page.Values.Count;
                        continue;
                    }

                    list.AddRange(page.Values.Take(specification.Take ?? 100).Select(x => x.ParseFromTableEntity()));
                    break;
                }

                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }

        public async Task<List<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        {
            var list = new List<TResult>();
            if (specification.Selector == null)
            {
                return list;
            }

            var selector = specification.Selector.Compile();
            var mappedFilter = _mapper.Map<Expression<Func<TMappingEntity, bool>>>(specification.WhereExpressions.FirstOrDefault()?.Filter ?? (x => true));
            var filter = AddPartitionKeyLambda(mappedFilter, PartitionKey);

            var query = _tableClient.QueryAsync(filter, specification.Take, cancellationToken: cancellationToken);

            var skip = 0;

            await foreach (var page in query.AsPages(pageSizeHint: specification.Take))
            {
                if (skip != (specification.Skip ?? 0))
                {
                    skip += page.Values.Count;
                    continue;
                }

                list.AddRange(page.Values
                    .Take(specification.Take ?? 100)
                    .Select(x => selector(x.ParseFromTableEntity())));
                break;
            }

            return list;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> SingleOrDefaultAsync(ISingleResultSpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var m = new TMappingEntity().SerializeToTableEntity(entity, PartitionKey);
            try
            {
                await _tableClient.UpdateEntityAsync(
                entity: m,
                ifMatch: m.ETag,
                cancellationToken: cancellationToken);
            }
            catch (RequestFailedException ex) when (ex.Status == 412)
            {
                throw new AppPreconditionFailedException(true, "Failed to update, because item was changed before", ex);
            }
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var mapped = entities.Select(x =>
            {
                var entity = new TMappingEntity().SerializeToTableEntity(x, PartitionKey);
                return new TableTransactionAction(TableTransactionActionType.UpdateMerge, entity, entity.ETag);
            });
            
            try
            {
                await _tableClient.SubmitTransactionAsync(mapped, cancellationToken: cancellationToken);
            }
            catch (RequestFailedException ex) when (ex.Status == 412)
            {
                throw new AppPreconditionFailedException(true, "Failed to update, because some of items was changed before", ex);
            }
        }
    }
}
