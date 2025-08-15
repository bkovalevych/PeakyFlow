using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Application.Common.Extensions;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.GameMapRules.GetMapRulesForRoom;
using PeakyFlow.Infrastructure.AzureTable;
using PeakyFlow.Infrastructure.AzureTable.Models;
using PeakyFlow.Infrastructure.Services;
using PeakyFlow.Infrastructure.SpreadSheets;
using Redis.OM;
using StackExchange.Redis;

namespace PeakyFlow.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IHostApplicationBuilder host, IConfiguration configuration)
        {
            host.AddAzureTableClient("peakytables");

            return host.Services
                .Configure<SheetsSettings>(configuration.GetSection(nameof(SheetsSettings)))
                .AddTransient<IGuid, IdService>()
                .AddTransient(x => new RedisConnectionProvider(x.GetRequiredService<IConnectionMultiplexer>()))
                .AddTransient<IDateProvider, DateProvider>()
                .AddScoped(typeof(IReadRepository<>), typeof(SheetsRepository<>))
                .RegisterAllImplementations(typeof(ISheetsRetriever<>), ServiceLifetime.Scoped)
                .AddScoped<IGetMapRulesForRoomService, GetMapRulesForRoomService>()
                //.AddScoped<IRepository<Lobby>, RedisRepository<Lobby, LobbyM>>()
                //.AddScoped<IRepository<Room>, RedisRepository<Room, RoomM>>()
                //.AddScoped<IRepository<GameMap>, RedisRepository<GameMap, GameMapM>>()
                //.AddScoped<IRepository<RoomState>, RedisRepository<RoomState, RoomStateM>>()
                .AddScoped<IRepository<Room>, AzureTableRepository<Room, RoomTableEntity>>()
                .AddScoped<IRepository<GameMap>, AzureTableRepository<GameMap, GameMapTableEntity>>()
                .AddScoped<IRepository<RoomState>, AzureTableRepository<RoomState, RoomStateTableEntity>>()
                .AddScoped<IRepository<Lobby>, AzureTableRepository<Lobby, LobbyTableEntity>>();
        }
    }
}
