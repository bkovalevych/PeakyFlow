using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeakyFlow.Abstractions.GameCardRuleAggregate;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.GameMapRuleAggregate;
using PeakyFlow.Abstractions.GameRoleAggregate;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Application.Common.Extensions;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.GameMapRules.GetMapRulesForRoom;
using PeakyFlow.Infrastructure.Redis;
using PeakyFlow.Infrastructure.Redis.Models;
using PeakyFlow.Infrastructure.Services;
using PeakyFlow.Infrastructure.SpreadSheets;
using Redis.OM;
using StackExchange.Redis;

namespace PeakyFlow.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<SheetsSettings>(configuration.GetSection(nameof(SheetsSettings)))
                .AddAutoMapper(conf => conf.AddExpressionMapping(), typeof(ServiceCollectionExtensions))
                .AddTransient<IGuid, IdService>()
                .AddHostedService<RedisHostedService>()
                .AddTransient(_ => new RedisConnectionProvider(new ConfigurationOptions()
                {
                    DefaultDatabase = 0,
                    EndPoints = { "localhost:6379" },
                    Ssl = false,
                    ConnectTimeout = 5000,
                    AbortOnConnectFail = false
                }))
                .AddTransient<IDateProvider, DateProvider>()
                .AddScoped(typeof(IReadRepository<>), typeof(SheetsRepository<>))
                .RegisterAllImplementations(typeof(ISheetsRetriever<>), ServiceLifetime.Scoped)
                .AddScoped<IGetMapRulesForRoomService, GetMapRulesForRoomService>()
                .AddScoped<IRepository<Lobby>, RedisRepository<Lobby, LobbyM>>()
                .AddScoped<IRepository<Room>, RedisRepository<Room, RoomM>>()
                .AddScoped<IRepository<GameMap>, RedisRepository<GameMap, GameMapM>>()
                .AddScoped<IRepository<GameMapRule>, RedisRepository<GameMapRule, GameMapRuleM>>()
                .AddScoped<IRepository<RoomState>, RedisRepository<RoomState, RoomStateM>>();
        }
    }
}
