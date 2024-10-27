using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace PeakyFlow.Application.Common.Behaviors
{
    internal class ExceptionHandlerBehavior<TRequest, TResponse>([FromKeyedServices("default")] ResiliencePipeline resiliencePipeline) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            return await resiliencePipeline.ExecuteAsync(async _ => await next());
        }
    }
}
