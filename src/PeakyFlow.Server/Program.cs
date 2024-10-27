using AutoMapper.Extensions.ExpressionMapping;
using PeakyFlow.Application.Common.Extensions;
using PeakyFlow.Application.LobbyGame;
using PeakyFlow.Infrastructure.Extensions;
using PeakyFlow.Infrastructure.SpreadSheets;
using PeakyFlow.Server.Common.Interfaces;
using PeakyFlow.Server.Interceptors;
using PeakyFlow.Server.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc(conf => conf.Interceptors.Add<ExceptionHanlerInterceptor>());
builder.Services.AddAutoMapper(conf => conf.AddExpressionMapping(), Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(SheetsSettings)), Assembly.GetAssembly(typeof(LobbyDto)));
builder.Services.AddMediatR(conf => conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.AddInfrastructure(builder.Configuration);




builder.Services.AddApplication();
builder.Services.AddScoped<LobbyGrpcEventReceiver>();
builder.Services.AddSingleton(typeof(INotificationReceiver<>), typeof(NotificationReceiver<>));
builder.AddServiceDefaults();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGrpcService<LobbyGrpcService>();
app.MapGrpcService<GameGrpcService>();

app.MapDefaultEndpoints();
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
