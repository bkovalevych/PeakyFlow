using MediatR;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.Common.Behaviors
{
    public class CacheBehavior<TRequest, TResponse>(ICacheService _cacheService) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICacheQuery
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_cacheService.TryGetValue(request.CacheKey, out TResponse cachedResponse))
            {
                return cachedResponse;
            }

            var response = await next();

            _cacheService.SetFor(request.CacheKey, response, request.MaxAge);

            return response;
        }
    }
}
