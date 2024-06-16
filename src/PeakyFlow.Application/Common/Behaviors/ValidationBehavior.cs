using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Reflection;

namespace PeakyFlow.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> _validators, IMapper _mapper) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = _mapper.Map<IEnumerable<ValidationFailure>, IEnumerable<ValidationError>>(validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors));

            if (!failures.Any())
            {
                return await next();
            }

            var resultGenericType = typeof(TResponse).GetGenericArguments().FirstOrDefault();

            if (resultGenericType == null)
            {
                return await next();
            }

            var resultTypeDefinition = typeof(Result<>).MakeGenericType(resultGenericType);
            var method = resultTypeDefinition
                        .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                        .FirstOrDefault(m => m.Name == "Invalid" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(IEnumerable<ValidationError>));
                
            if (method == null)
            {
                return await next();
            }

            var result = (TResponse)(object)method?.Invoke(null, [failures]);
            if (result == null)
            {
                return await next();
            }

            return result;            
        }
   }
}