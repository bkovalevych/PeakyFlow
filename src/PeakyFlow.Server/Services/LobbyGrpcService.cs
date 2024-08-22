using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.LobbyGame.CloseLobby;
using PeakyFlow.Application.LobbyGame.CloseLobbyAndStartGame;
using PeakyFlow.Application.LobbyGame.Create;
using PeakyFlow.GrpcProtocol.Common;
using PeakyFlow.GrpcProtocol.Lobby;
using PeakyFlow.Server.Common.Extensions;
using PeakyFlow.Server.Common.Interfaces;

namespace PeakyFlow.Server.Services
{
    public class LobbyGrpcService(
        INotificationReceiver<LobbyCreatedEvent> lobbyCreatedReceiver,
        INotificationReceiver<LobbyClosedEvent> lobbyClosedReceiver,
        INotificationReceiver<LobbyClosedAndGameStartedEvent> lobbyClosedAndGameStartedReceiver,
        IMediator mediator,
        IMapper mapper) : LobbyRpcService.LobbyRpcServiceBase
    {

        public override async Task<CreateLobbyResp> CreateLobby(CreateLobbyMessage request, ServerCallContext context)
        {
            var command = new CreateLobbyCommand(request.Owner, request.Name, request.TeamSize, request.Password);

            var result = await mediator.Send(command, context.CancellationToken);

            var resp = new CreateLobbyResp();

            resp.BaseResp = result.ToRespBase(mapper);
            resp.Id = result.Value;

            return resp;
        }

        public override async Task<RespBase> CloseLobby(CloseLobbyMessage request, ServerCallContext context)
        {
            var command = new CloseLobbyCommand(request.LobbyId, request.PlayerId);

            var result = await mediator.Send(command, context.CancellationToken);

            var resp = result.ToRespBase(mapper);

            return resp;
        }

        public override async Task<CloseLobbyAndStartGameResp> CloseLobbyAndStartGame(
            CloseLobbyAndStartGameMessage request, ServerCallContext context)
        {
            var command = new CloseLobbyAndStartGameCommand(request.LobbyId);

            var result = await mediator.Send(command, context.CancellationToken);

            var resp = mapper.Map<CloseLobbyAndStartGameResp>(result.Value);

            resp.BaseResp = result.ToRespBase(mapper);
           
            return resp;
        }

        public override async Task OnLobbyListEvent(Empty request, IServerStreamWriter<LobbyListEvent> responseStream, ServerCallContext context)
        {
            lobbyCreatedReceiver.ReceiveNotifications()
                .Subscribe(async lobby => 
                {
                    var eventInstance = new LobbyListEvent()
                    {
                        Added = mapper.Map<LobbyItem>(lobby)
                    };
                    await responseStream.WriteAsync(eventInstance, context.CancellationToken);
                }, context.CancellationToken);
            
            lobbyClosedAndGameStartedReceiver.ReceiveNotifications()
                .Subscribe(async lobbyClosed =>
                {
                    var eventInstance = new LobbyListEvent()
                    {
                        ClosedId = lobbyClosed.LobbyId
                    };
                    await responseStream.WriteAsync(eventInstance, context.CancellationToken);
                }, context.CancellationToken);

            lobbyClosedReceiver.ReceiveNotifications()
                .Subscribe(async lobbyClosed =>
                {
                    var eventInstance = new LobbyListEvent()
                    {
                        ClosedId = lobbyClosed.LobbyId
                    };

                    await responseStream.WriteAsync(eventInstance, context.CancellationToken);
                }, context.CancellationToken);

            try
            {
                await Task.Delay(Timeout.Infinite, context.CancellationToken);
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
