using Blazored.SessionStorage;
using Grpc.Core;
using PeakyFlow.Aspire.Web.Components.Models;
using PeakyFlow.GrpcProtocol.Lobby;

namespace PeakyFlow.Aspire.Web.Components.Services
{
    public class JoinLobbyService : IDisposable
    {
        private readonly LobbyRpcService.LobbyRpcServiceClient _lobbyRpcService;
        private readonly ISessionStorageService _sessionStorageService;
        private readonly CancellationTokenSource _cancellationTokenService;
        private readonly ILogger<JoinLobbyService> _logger;

        public event Action<LobbyEvent>? OnLobbyEvent;

        private const string LobbyStateKey = "PlayerInLobby";

        public LobbyMsg? Lobby { get; private set; }
        public LobbyPlayerMsg? Player => Lobby?.Players.FirstOrDefault(x => State != null && x.Id == State.PlayerId);
        public JoinedLobbyState? State { get; private set; }

        public JoinLobbyService(LobbyRpcService.LobbyRpcServiceClient 
            lobbyRpcService,
            ISessionStorageService sessionStorageService,
            ILogger<JoinLobbyService> logger)
        {
            _lobbyRpcService = lobbyRpcService;
            _sessionStorageService = sessionStorageService;
            _cancellationTokenService = new CancellationTokenSource();
            _logger = logger;
        }

        public async Task<JoinLobbyResp> JoinLobby(string lobbyId, string playerName, string? password = null)
        {
            var resp = await _lobbyRpcService.JoinLobbyAsync(new JoinLobbyMessage()
            {
                LobbyId = lobbyId,
                Password = password,
                PlayerName = playerName
            });

            if (resp.Successfully)
            {
                await _sessionStorageService.SetItemAsync(LobbyStateKey, new JoinedLobbyState()
                {
                    PlayerId = resp.PlayerId,
                    LobbyId = lobbyId
                });
            }
            
            return resp;
        }

        public async Task OnInit()
        {
            State = await _sessionStorageService.GetItemAsync<JoinedLobbyState>(LobbyStateKey);

            if (State == null)
            {
                throw new Exception("Player in lobby was not found");
            }

            var resp = await _lobbyRpcService.GetLobbyAsync(new GetLobbyMsg()
            {
                LobbyId = State.LobbyId,
                PlayerId = State.PlayerId
            });

            if (resp.BaseResp.Status == GrpcProtocol.Common.ResultStatusMsg.Ok)
            {
                Lobby = resp.Lobby;
            }
            else
            {
                _logger.LogError("Error on init. Response: {response}", resp.BaseResp.Status);
            }
        }

        public async Task LeaveLobby()
        {
            var resp = await _lobbyRpcService.LeaveLobbyAsync(new LeaveLobbyMessage()
            {
                LobbyId = State.LobbyId,
                PlayerId= State.PlayerId
            });

            await _sessionStorageService.RemoveItemAsync(LobbyStateKey);
        }

        public async Task SetMeAsReady(bool isReady)
        {
            await _lobbyRpcService.PlayerIsReadyAsync(new PlayerIsReadyMessage()
            { 
                LobbyId = State.LobbyId,
                PlayerId = State.PlayerId,
                IsReady = isReady
            });
        }

        public async void SubscribeOnLobbyEvents()
        {
            try
            {
                var call = _lobbyRpcService.OnLobbyEvent(new LobbyEventMessage()
                {
                    LobbyId = State.LobbyId
                });

                while (await call.ResponseStream.MoveNext(_cancellationTokenService.Token))
                {
                    OnLobbyEvent?.Invoke(call.ResponseStream.Current);
                }
            }
            catch (RpcException cancelled) when (cancelled.StatusCode == StatusCode.Cancelled)
            {
                _logger.LogWarning(cancelled, "Cancelled");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Unexpected Error occured");
            }
        }

        public void Dispose()
        {
            _cancellationTokenService.Dispose();
        }
    }
}
