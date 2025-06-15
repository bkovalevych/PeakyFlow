using Blazored.SessionStorage;
using Grpc.Core;
using PeakyFlow.Aspire.Web.Components.Models;
using PeakyFlow.GrpcProtocol.Lobby;

namespace PeakyFlow.Aspire.Web.Components.Services
{
    public class CreateLobbyAndStartGameService
        (LobbyRpcService.LobbyRpcServiceClient lobbyRpc,
        ISessionStorageService sessionStorageService,
        ILogger<CreateLobbyAndStartGameService> logger) 
        : IDisposable
    {
        public LobbyMsg? CreatedLobby { get; private set; }
        private const string LobbyStateKey = "createdLobbyState";
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public event Action<LobbyEvent>? OnLobbyEvent;

        public async Task<CreateLobbyResp?> CreateLobby(CreateLobbyMessage msg)
        {
            var res = await lobbyRpc.CreateLobbyAsync(msg);
            var created = res != null && res.BaseResp.Status == GrpcProtocol.Common.ResultStatusMsg.Ok
                 && res.Lobby != null;
            
            if (res?.Lobby != null && created) 
            {
                CreatedLobby = res.Lobby;
                await sessionStorageService.SetItemAsync(LobbyStateKey, 
                    new OwnedLobbyState()
                    {
                        LobbyId = res.Lobby.Id, 
                        OwnerId = res.Lobby.Players.First(x => x.IsOwner).Id,
                    });
            }
            
            return res;
        }

        public async Task OnInit()
        {
            var lobbyState = await sessionStorageService.GetItemAsync<OwnedLobbyState>(LobbyStateKey);

            if (lobbyState == null)
            {
                throw new Exception("lobbyState was not found");
            }
            
            var resp = await lobbyRpc.GetLobbyAsync(new GetLobbyMsg() { LobbyId = lobbyState.LobbyId, PlayerId = lobbyState.OwnerId });

            if (resp.Lobby == null) 
            {
                throw new Exception($"getting lobby failed with status {resp.BaseResp.Status}");
            }

            CreatedLobby = resp.Lobby;
        }

        public async void SubscribeOnLobbyEvents()
        {
            try
            {
                var call = lobbyRpc.OnLobbyEvent(new LobbyEventMessage() { LobbyId = CreatedLobby.Id });
                
                while (await call.ResponseStream.MoveNext(_cancellationTokenSource.Token))
                {
                    OnLobbyEvent?.Invoke(call.ResponseStream.Current);
                }
            }
            catch (RpcException cancelled) when (cancelled.StatusCode == StatusCode.Cancelled)
            {
                logger.LogWarning(cancelled, "Cancelled");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed on lobby event");
            }
        }

        public async Task CancelLobby()
        {
            await lobbyRpc.CloseLobbyAsync(new CloseLobbyMessage()
            {
                LobbyId = CreatedLobby?.Id
            });

            await sessionStorageService.RemoveItemAsync(LobbyStateKey);
        }

        public async Task CloseLobbyAndStartGame()
        {
            await lobbyRpc.CloseLobbyAndStartGameAsync(new CloseLobbyAndStartGameMessage()
            {
                LobbyId = CreatedLobby?.Id,
                PlayerId = CreatedLobby?.Owner
            });
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
