// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PeakyFlow.Application.Common.Extensions;
using PeakyFlow.Application.GameMapRules.GetMapRulesForRoom;
using PeakyFlow.Console.Services;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services
            //.AddLogging(x => x.AddDebug().AddConsole().SetMinimumLevel(LogLevel.Information))
            .AddApplication()
            .AddSingleton<MainService>()
            .AddTransient<IGetMapRulesForRoomService, GetMapRulesService>();
    })
    .Build();

host.StartAsync().Wait();
var main = host.Services.GetRequiredService<MainService>();

main.StartMap();

Console.WriteLine("Press any key to quit...");
Console.ReadKey();
host.StopAsync().Wait();