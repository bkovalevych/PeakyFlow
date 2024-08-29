using Ardalis.Specification;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Options;
using PeakyFlow.Abstractions;
using PeakyFlow.Application.Common.Interfaces;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource.BatchGetRequest;

namespace PeakyFlow.Infrastructure.SpreadSheets
{
    public class SheetsRepository<TEntity> : IReadRepository<TEntity>
        where TEntity : Entity, IAggregateRoot
    {
        private readonly SheetsService _sheetsService;
        private readonly ISheetsRetriever<TEntity> _sheetsRetriever;
        private readonly SheetsSettings _sheetsSettings;
        private List<TEntity>? _entities;

        public SheetsRepository(IOptions<SheetsSettings> delegateSheets,
            ISheetsRetriever<TEntity> sheetsRetriever)
        {
            _sheetsRetriever = sheetsRetriever;
            _sheetsSettings = delegateSheets.Value;

            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                ApiKey = delegateSheets.Value.ApiKey,
                ApplicationName = delegateSheets.Value.ApplicationName
            });
        }

        private async Task<List<TEntity>> InitIfNotInited()
        {
            var request = _sheetsService.Spreadsheets.Values.BatchGet(
                _sheetsSettings.SheetId);
            
            request.Ranges = _sheetsRetriever.Ranges;
            
            request.ValueRenderOption = ValueRenderOptionEnum.FORMATTEDVALUE;
            var val = await request.ExecuteAsync();
            
            var result = _sheetsRetriever.Retrieve(val.ValueRanges.Select(x => x.Values).ToList());
            return result;
        }

        public async Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();

            IEnumerable<TEntity> initial = _entities;

            foreach (var t in specification.WhereExpressions)
            {
                initial = initial.Where(t.FilterFunc);
            }

            return initial.Any();
        }

        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();
            return _entities.Any();
        }

        public async IAsyncEnumerable<TEntity> AsAsyncEnumerable(ISpecification<TEntity> specification)
        {
            _entities ??= await InitIfNotInited();
            
            foreach (var t in _entities)
            {
                yield return t;
            }
        }

        public async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();

            IEnumerable<TEntity> initial = _entities;

            foreach (var t in specification.WhereExpressions)
            {
                initial = initial.Where(t.FilterFunc);
            }

            return initial.Count();
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();
            return _entities.Count();
        }

        public async Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();

            IEnumerable<TEntity> initial = _entities;

            foreach (var t in specification.WhereExpressions)
            {
                initial = initial.Where(t.FilterFunc);
            }

            return initial.FirstOrDefault();
        }

        public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();
            IEnumerable<TEntity> initial = _entities;

            foreach (var t in specification.WhereExpressions)
            {
                initial = initial.Where(t.FilterFunc);
            }

            var s = specification.Selector?.Compile() ?? throw new ArgumentException("Selector was not specified");
            return initial.Select(s)
                .FirstOrDefault();
        }

        public async Task<TEntity?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
        {
            _entities ??= await InitIfNotInited();

            if (typeof(TId) != typeof(string))
            {
                throw new ArgumentException("id should me string");
            }

            var strId = id as string;

            return _entities.FirstOrDefault(x => x.Id == strId);
        }

        public async Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();

            IEnumerable<TEntity> initial = _entities;

            foreach (var t in specification.WhereExpressions)
            {
                initial = initial.Where(t.FilterFunc);
            }

            return initial.FirstOrDefault();
        }

        public async Task<TResult?> GetBySpecAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();

            IEnumerable<TEntity> initial = _entities;

            foreach (var t in specification.WhereExpressions)
            {
                initial = initial.Where(t.FilterFunc);
            }

            var s = specification.Selector?.Compile() ?? throw new ArgumentException("Selector was not specified");
            return initial.Select(s)
                .FirstOrDefault();
        }

        public async Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();

            return _entities;
        }

        public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();
            IEnumerable<TEntity> initial = _entities;

            foreach (var t in specification.WhereExpressions)
            {
                initial = initial.Where(t.FilterFunc);
            }

            return initial.ToList();
        }

        public async Task<List<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();

            IEnumerable<TEntity> initial = _entities;

            foreach (var t in specification.WhereExpressions)
            {
                initial = initial.Where(t.FilterFunc);
            }

            var s = specification.Selector?.Compile() ?? throw new ArgumentException("Selector was not specified");
            
            return initial.Select(s)
                .ToList();
        }

        public async Task<TEntity?> SingleOrDefaultAsync(ISingleResultSpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();

            IEnumerable<TEntity> initial = _entities;

            foreach (var t in specification.WhereExpressions)
            {
                initial = initial.Where(t.FilterFunc);
            }

            return initial.SingleOrDefault();
        }

        public async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        {
            _entities ??= await InitIfNotInited();

            IEnumerable<TEntity> initial = _entities;

            foreach (var t in specification.WhereExpressions)
            {
                initial = initial.Where(t.FilterFunc);
            }

            var s = specification.Selector?.Compile() ?? throw new ArgumentException("Selector was not specified");

            return initial.Select(s)
                .SingleOrDefault();
        }
    }
}
