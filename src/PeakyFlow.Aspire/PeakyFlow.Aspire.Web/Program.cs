using Blazored.SessionStorage;
using PeakyFlow.Aspire.Web.Components;
using PeakyFlow.Aspire.Web.Components.Services;
using PeakyFlow.GrpcProtocol.Game;
using PeakyFlow.GrpcProtocol.Lobby;
using Radzen;
//using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
    //.AddInteractiveWebAssemblyComponents();

builder.Services.AddRadzenComponents();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<CreateLobbyAndStartGameService>();
builder.Services.AddLogging();

builder.Services.AddGrpcClient<LobbyRpcService.LobbyRpcServiceClient>((x) =>
{
    x.Address = new(builder.Configuration["services__server__https__0"] ?? builder.Configuration["services:server:https:0"] ?? string.Empty);
})
.ConfigureChannel((s, ch) =>
{
    ch.LoggerFactory = s.GetRequiredService<ILoggerFactory>();
});

builder.Services.AddGrpcClient<GameRpcService.GameRpcServiceClient>(o =>
{
    o.Address = new(builder.Configuration["services__server__https__0"] ?? builder.Configuration["services:server:https:0"] ?? string.Empty);
})
.ConfigureChannel((s, ch) =>
{
    ch.LoggerFactory = s.GetRequiredService<ILoggerFactory>();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
    //.AddInteractiveWebAssemblyRenderMode();

app.MapDefaultEndpoints();

app.Run();
