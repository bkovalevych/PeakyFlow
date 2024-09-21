// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeakyFlow.Console.Services;
using PeakyFlow.GrpcProtocol.Game;
using PeakyFlow.GrpcProtocol.Lobby;

var host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(conf =>
        conf.AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
    )
    .ConfigureServices((context, services) =>
    {
        services
        //.AddLogging(x => x.AddDebug().AddConsole().SetMinimumLevel(LogLevel.Information))
        .AddSingleton<MainService>()
        .AddGrpcClient<LobbyRpcService.LobbyRpcServiceClient>(conf =>
        {
            conf.Address = new Uri(context.Configuration.GetConnectionString("Server")!);
        });
        services.AddGrpcClient<GameRpcService.GameRpcServiceClient>(conf =>
        {
            conf.Address = new Uri(context.Configuration.GetConnectionString("Server")!);
        });
    })
    .Build();

await host.StartAsync();
var main = host.Services.GetRequiredService<MainService>();

main.StartMap();

Console.WriteLine("Press any key to quit...");
Console.ReadKey();
host.StopAsync().Wait();