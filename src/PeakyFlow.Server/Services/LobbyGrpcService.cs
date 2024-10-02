using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.LobbyGame.CloseLobby;
using PeakyFlow.Application.LobbyGame.CloseLobbyAndStartGame;
using PeakyFlow.Application.LobbyGame.Create;
using PeakyFlow.Application.LobbyGame.GetLobby;
using PeakyFlow.Application.LobbyGame.JoinLobby;
using PeakyFlow.Application.LobbyGame.LeaveLobby;
using PeakyFlow.Application.LobbyGame.List;
using PeakyFlow.Application.LobbyGame.PlayerInLobbySetIsReady;
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
        LobbyGrpcEventReceiver lobbyGrpcEventReceiver,
        IMediator mediator,
        IMapper mapper) : LobbyRpcService.LobbyRpcServiceBase
    {
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

            var resp = mapper.Map<CloseLobbyAndStartGameResp>(result.Value) ?? new CloseLobbyAndStartGameResp();

            resp.BaseResp = result.ToRespBase(mapper);
           
            return resp;
        }

        public override async Task<CreateLobbyResp> CreateLobby(CreateLobbyMessage request, ServerCallContext context)
        {
            var command = new CreateLobbyCommand(request.Owner, request.Name, request.TeamSize, request.Password);

            var result = await mediator.Send(command, context.CancellationToken);

            var resp = new CreateLobbyResp();

            resp.BaseResp = result.ToRespBase(mapper);
            resp.Lobby = mapper.Map<LobbyMsg>(result.Value);

            return resp;
        }

        public override async Task<JoinLobbyResp> JoinLobby(JoinLobbyMessage request, ServerCallContext context)
        {
            var command = new JoinLobbyCommand(request.LobbyId, request.PlayerName, request.Password);

            var result = await mediator.Send(command, context.CancellationToken);

            var resp = mapper.Map<JoinLobbyResp>(result.Value) ?? new JoinLobbyResp();

            resp.BaseResp = result.ToRespBase(mapper);

            return resp;
        }

        public override async Task<RespBase> LeaveLobby(LeaveLobbyMessage request, ServerCallContext context)
        {
            var command = new LeaveLobbyCommand(request.LobbyId, request.PlayerId);

            var result = await mediator.Send(command, context.CancellationToken);

            var resp = result.ToRespBase(mapper);

            return resp;
        }

        public override async Task<GetLobbyResp> GetLobby(GetLobbyMsg request, ServerCallContext context)
        {
            var query = new GetLobbyQuery(request.LobbyId, request.PlayerId);

            var result = await mediator.Send(query, context.CancellationToken);

            var resp = new GetLobbyResp();
            resp.Lobby = mapper.Map<LobbyMsg>(result.Value);
            resp.BaseResp = result.ToRespBase(mapper);

            return resp;
        }

        public override async Task<LobbyListResp> ListLobbies(ListLobbyMessage request, ServerCallContext context)
        {
            var command = new LobbyListQuery()
            {
                PaginationCount = request.PaginationCount,
                PaginationSkip = request.PaginationSkip
            };

            var result = await mediator.Send(command, context.CancellationToken);

            var resp = new LobbyListResp();
            resp.Lobbies.AddRange(mapper.Map<IEnumerable<LobbyItem>>(result.Value) ?? Enumerable.Empty<LobbyItem>());

            resp.BaseResp = result.ToRespBase(mapper);

            return resp;
        }

        public override async Task<RespBase> PlayerIsReady(PlayerIsReadyMessage request, ServerCallContext context)
        {
            var command = new PlayerInLobbySetIsReadyCommand(request.LobbyId, request.PlayerId, request.IsReady);

            var result = await mediator.Send(command, context.CancellationToken);

            var resp = result.ToRespBase(mapper);

            return resp;
        }

        public override async Task OnLobbyEvent(LobbyEventMessage request, IServerStreamWriter<LobbyEvent> responseStream, ServerCallContext context)
        {
            await foreach (var eventInst in lobbyGrpcEventReceiver
                .ReceiveLobbyEvents(request.LobbyId, context.CancellationToken))
            {
                await responseStream.WriteAsync(eventInst, context.CancellationToken);
            }
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
