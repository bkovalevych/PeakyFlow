using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PeakyFlow.Abstractions.Common.Exceptions;
using PeakyFlow.Abstractions.Common.Interfaces;
using PeakyFlow.Abstractions.Common.Services;
using PeakyFlow.Application.Common.Behaviors;
using PeakyFlow.Application.LobbyGame.Create;
using PeakyFlow.Application.Roles.GetRoleForPlayer;
using Polly;
using Polly.Retry;
using System.Reflection;

namespace PeakyFlow.Application.Common.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services
                .AddResiliencePipeline("default", builder => builder.AddRetry(new RetryStrategyOptions()
                {
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromMilliseconds(5000),
                    MaxRetryAttempts = 2,
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder()
                    .Handle<AppPreconditionFailedException>(x => x.CanBeRetried)
                }))
                .AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlerBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(PlayerIsTakingTurnBehavior<,>))
                .AddTransient<IStringConverter, MyStringConverter>()
                .AddValidatorsFromAssemblyContaining<CreateLobbyValidator>()
                .AddTransient<IGetRoleForPlayerService, GetRoleForPlayerService>();
        }

        public static IServiceCollection RegisterAllImplementations(this IServiceCollection services, Type type, ServiceLifetime lifetime) 
        {
            var implementations = type.Assembly.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type)
                && !t.IsGenericType);
            
            foreach (var implementation in implementations)
            {
                foreach (var interfaceDefinition in implementation.GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == type))
                {
                    services.Add(new ServiceDescriptor(interfaceDefinition, implementation, lifetime));
                }
                
            }
            
            return services;
        }
    }
}
