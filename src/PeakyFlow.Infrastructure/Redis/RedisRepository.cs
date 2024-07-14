using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using PeakyFlow.Application.Common.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace PeakyFlow.Infrastructure.Redis
{
    public class RedisRepository<T> : IRepository<T>, IDisposable
        where T : class
    {
        private bool disposedValue;
        private readonly ILogger<RedisRepository<T>> _logger;
        private readonly RedisConnectionProvider _redisProvider;
        private readonly RedisCollection<T> _redisCollection;

        public RedisRepository(RedisConnectionProvider provider, ILogger<RedisRepository<T>> logger)
        {
            _redisProvider = provider;
            _redisCollection = (RedisCollection<T>)_redisProvider.RedisCollection<T>();

            _logger = logger;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _redisCollection.InsertAsync(entity);

            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _redisCollection.InsertAsync(entities);
            return entities;
        }

        public Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            IRedisCollection<T> initialExpression = _redisCollection;

            foreach (var where in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(where.Filter);
            }

            return initialExpression.AnyAsync();
        }

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return _redisCollection.AnyAsync();
        }

        public async IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
        {
            await foreach (var t in _redisCollection)
            {
                yield return t;
            }
        }

        public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            IRedisCollection<T> initialExpression = _redisCollection;

            foreach (var where in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(where.Filter);
            }

            return initialExpression.CountAsync();
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return _redisCollection.CountAsync();
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            return _redisCollection.DeleteAsync(entity);
        }

        public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            return _redisCollection.DeleteAsync(entities);
        }

        public async Task DeleteRangeAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            IRedisCollection<T> initialExpression = _redisCollection;
            foreach (var where in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(where.Filter);
            }

            await _redisCollection.DeleteAsync(initialExpression);
        }

        public Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            IRedisCollection<T> initialExpression = _redisCollection;

            foreach (var where in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(where.Filter);
            }

            return initialExpression.FirstOrDefaultAsync();
        }

        public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            if (specification.Selector == null)
            {
                throw new ArgumentException("Selector was not defined for first or default {entity}", nameof(T));
            }

            foreach (var where in specification.WhereExpressions)
            {
                var pre = await _redisCollection.FirstOrDefaultAsync(where.Filter);

                if (pre != null)
                {
                    return specification.Selector.Compile()(pre);
                }
            }

            return default;
        }

        public Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
        {
            if (typeof(TId) != typeof(string))
            {
                throw new ArgumentException("Type of id was not a string");
            }

            return _redisCollection.FindByIdAsync(id.ToString() ?? string.Empty);
        }

        public Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return FirstOrDefaultAsync(specification, cancellationToken);
        }

        public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            return FirstOrDefaultAsync(specification, cancellationToken);
        }

        public async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
        {
            return (await _redisCollection.ToListAsync()).ToList();
        }

        public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            var result = new List<T>();

            foreach (var where in specification.WhereExpressions)
            {
                result.AddRange(await _redisCollection.Where(where.Filter).ToListAsync());
            }

            return result;
        }

        public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            if (specification.Selector == null)
            {
                throw new ArgumentException("selector was not specified for entity {entity}", nameof(T));
            }

            IRedisCollection<T> initialExpression = _redisCollection;

            foreach (var where in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(where.Filter);
            }


#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
            return [.. (await initialExpression.Select(specification.Selector).ToListAsync())];
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _redisCollection.SaveAsync();
            //TODO implement transaction
            return 0;
        }

        public Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
        {
            IRedisCollection<T> initialExpression = _redisCollection;

            foreach (var t in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(t.Filter);
            }

            return initialExpression.SingleOrDefaultAsync();
        }

        public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            if (specification.Selector == null)
            {
                throw new ArgumentException("Selector was null for single or default");
            }

            IRedisCollection<T> initialExpression = _redisCollection;

            foreach (var t in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(t.Filter);
            }


#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
            return initialExpression.Select(specification.Selector).SingleOrDefaultAsync();
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            return _redisCollection.UpdateAsync(entity);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _redisCollection.UpdateAsync(entities);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _redisProvider.Connection.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
