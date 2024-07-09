using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PeakyFlow.Application.Common.Behaviors;
using PeakyFlow.Application.LobbyGame.Create;
using PeakyFlow.Application.Roles.GetRoleForPlayer;
using System.Reflection;

namespace PeakyFlow.Application.Common.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services
                .AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheBehavior<,>))
                .AddAutoMapper(Assembly.GetExecutingAssembly())
                .AddValidatorsFromAssemblyContaining<CreateLobbyValidator>()
                .AddTransient<IGetRoleForPlayerService, GetRoleForPlayerService>();
        }
    }
}
