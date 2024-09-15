using Ardalis.Specification;
using AutoMapper;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Infrastructure.Redis.Models;
using Redis.OM;
using Redis.OM.Searching;
using System.Linq.Expressions;

namespace PeakyFlow.Infrastructure.Redis
{
    public class RedisRepository<T, TModel> : IRepository<T>, IReadRepository<T>, IDisposable
        where T : Entity
        where TModel : EntityM
    {
        public const int MaxTake = 100;
        private bool disposedValue;
        private readonly ILogger<RedisRepository<T, TModel>> _logger;
        private readonly RedisConnectionProvider _redisProvider;
        private readonly RedisCollection<TModel> _redisCollection;
        private readonly IMapper _mapper;

        public RedisRepository(RedisConnectionProvider provider, IMapper mapper, ILogger<RedisRepository<T, TModel>> logger)
        {
            _redisProvider = provider;
            _redisCollection = (RedisCollection<TModel>)_redisProvider.RedisCollection<TModel>(false);
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _redisCollection.InsertAsync(_mapper.Map<TModel>(entity));

            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _redisCollection.InsertAsync(_mapper.Map<IEnumerable<TModel>>(entities));
            return entities;
        }

        public Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            IRedisCollection<TModel> initialExpression = _redisCollection;

            foreach (var where in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(_mapper.Map<Expression<Func<TModel, bool>>>(where.Filter));
            }

            return initialExpression.AnyAsync();
        }

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return _redisCollection.AnyAsync();
        }

        public async IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
        {
            await foreach (var t in _redisCollection.Skip(specification.Skip ?? 0)
                .Take(specification.Take ?? MaxTake))
            {
                yield return _mapper.Map<T>(t);
            }
        }

        public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            IRedisCollection<TModel> initialExpression = _redisCollection;

            foreach (var where in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(_mapper.Map<Expression<Func<TModel, bool>>>(where.Filter));
            }

            return initialExpression.CountAsync();
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return _redisCollection.CountAsync();
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            return _redisCollection.DeleteAsync(_mapper.Map<TModel>(entity));
        }

        public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            return _redisCollection.DeleteAsync(_mapper.Map<TModel>(entities));
        }

        public async Task DeleteRangeAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            IRedisCollection<TModel> initialExpression = _redisCollection;
            foreach (var where in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(_mapper.Map<Expression<Func<TModel, bool>>>(where.Filter));
            }

            await _redisCollection.DeleteAsync(initialExpression);
        }

        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            IRedisCollection<TModel> initialExpression = _redisCollection;

            foreach (var where in specification.WhereExpressions)
            {
                var filter = _mapper.Map<Expression<Func<TModel, bool>>>(where.Filter);
                initialExpression = initialExpression.Where(filter);
            }

            return _mapper.Map<T>(await initialExpression.FirstOrDefaultAsync());
        }

        public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            if (specification.Selector == null)
            {
                throw new ArgumentException("Selector was not defined for first or default {entity}", nameof(T));
            }

            foreach (var where in specification.WhereExpressions)
            {
                var pre = await _redisCollection.FirstOrDefaultAsync(_mapper.Map<Expression<Func<TModel, bool>>>(where.Filter));

                if (pre != null)
                {
                    return specification.Selector.Compile()(_mapper.Map<T>(pre));
                }
            }

            return default;
        }

        public async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
        {
            if (typeof(TId) != typeof(string))
            {
                throw new ArgumentException("Type of id was not a string");
            }

            return _mapper.Map<T>(await _redisCollection.FindByIdAsync(id.ToString() ?? string.Empty));
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
            return _mapper.Map<List<T>>(await _redisCollection.ToListAsync());
        }

        public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            IRedisCollection<TModel> initialExpression = _redisCollection;
            foreach (var where in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(_mapper.Map<Expression<Func<TModel, bool>>>(where.Filter));
            }

            return _mapper.Map<List<T>>(await initialExpression.Skip(specification.Skip ?? 0)
                .Take(specification.Take ?? MaxTake).ToListAsync());
        }

        public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            if (specification.Selector == null)
            {
                throw new ArgumentException("selector was not specified for entity {entity}", nameof(T));
            }

            IRedisCollection<TModel> initialExpression = _redisCollection;

            foreach (var where in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(_mapper.Map<Expression<Func<TModel, bool>>>(where.Filter));
            }

            var list = _mapper.Map<List<T>>(await initialExpression.Skip(specification.Skip ?? 0)
                .Take(specification.Take ?? MaxTake).ToListAsync());
            var selector = specification.Selector.Compile();

            return list.Select(x => selector(x)).ToList();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
            //await _redisCollection.SaveAsync();
            //TODO implement transaction
            //return 0;
        }

        public async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
        {
            IRedisCollection<TModel> initialExpression = _redisCollection;

            foreach (var t in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(_mapper.Map<Expression<Func<TModel, bool>>>(t.Filter));
            }

            return _mapper.Map<T>(await initialExpression.SingleOrDefaultAsync());
        }

        public async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            if (specification.Selector == null)
            {
                throw new ArgumentException("Selector was null for single or default");
            }

            IRedisCollection<TModel> initialExpression = _redisCollection;

            foreach (var t in specification.WhereExpressions)
            {
                initialExpression = initialExpression.Where(_mapper.Map<Expression<Func<TModel, bool>>>(t.Filter));
            }

            var selector = specification.Selector.Compile();

            return selector(_mapper.Map<T>(await initialExpression.SingleOrDefaultAsync()));
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var model = _mapper.Map<TModel>(entity);
            return _redisCollection.UpdateAsync(model);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            var models = _mapper.Map<IEnumerable<TModel>>(entities);

            await _redisCollection.UpdateAsync(models);
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
