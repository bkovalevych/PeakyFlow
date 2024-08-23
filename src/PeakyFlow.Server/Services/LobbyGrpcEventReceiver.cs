using AutoMapper;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.GrpcProtocol.Lobby;
using PeakyFlow.Server.Common.Interfaces;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace PeakyFlow.Server.Services
{
    public class LobbyGrpcEventReceiver(
        INotificationReceiver<LobbyClosedEvent> closedReceiver,
        INotificationReceiver<LobbyClosedAndGameStartedEvent> gameStartedReceiver,
        INotificationReceiver<PlayerLeftLobbyEvent> playerLeftReceiver,
        INotificationReceiver<PlayerJoinedEvent> playerJoinedReceiver,
        INotificationReceiver<PlayerInLobbyIsReadyEvent> playerIsReadyReceiver,
        IMapper mapper)
    {
        public async IAsyncEnumerable<LobbyEvent> ReceiveLobbyEvents(string lobbyId, [EnumeratorCancellation] CancellationToken ct)
        {
            var messages = Observable.Merge(
                closedReceiver.ReceiveNotifications()
                .Where(x => x.LobbyId == lobbyId)
                .Select(x => new LobbyEvent()
                {
                    LobbyClosedId = x.LobbyId
                }),
                gameStartedReceiver.ReceiveNotifications()
                .Where(x => x.LobbyId == lobbyId)
                .Select(x => new LobbyEvent()
                {
                    LobbyClosedAndGameStarted = mapper.Map<LobbyClosedAndGameStartedMessage>(x)
                }),
                playerLeftReceiver.ReceiveNotifications()
                .Where(x => x.RoomId == lobbyId)
                .Select(x => new LobbyEvent()
                {
                    PlayerLeftId = x.PlayerId
                }),
                playerJoinedReceiver.ReceiveNotifications()
                .Where(x => x.LobbyId == lobbyId)
                .Select(x => new LobbyEvent()
                {
                    PlayerJoined = mapper.Map<PlayerJoinedToLobbyMessage>(x)
                }),
                playerIsReadyReceiver.ReceiveNotifications()
                .Where(x => x.LobbyId == lobbyId && x.IsReady)
                .Select(x => new LobbyEvent()
                {
                    PlayerIsReadyId = x.PlayerId
                }),
                playerIsReadyReceiver.ReceiveNotifications()
                .Where(x => x.LobbyId == lobbyId && !x.IsReady)
                .Select(x => new LobbyEvent()
                {
                    PlayerIsNotReadyId = x.PlayerId
                }));

            await foreach (var message in messages.ToAsyncEnumerable())
            {
                yield return message;
                
                if (message.ReasonCase == LobbyEvent.ReasonOneofCase.LobbyClosedId
                    || message.ReasonCase == LobbyEvent.ReasonOneofCase.LobbyClosedAndGameStarted)
                {
                    yield break;
                }
            }
        }
    }
}
