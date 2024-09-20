using AutoMapper;
using Grpc.Core;
using MediatR;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameMapAggregate.Events;
using PeakyFlow.Abstractions.RoomAggregate.Events;
using PeakyFlow.Abstractions.RoomStateAggregate.Events;
using PeakyFlow.Application.GameMaps.EndTurn;
using PeakyFlow.Application.GameMaps.GetGameMap;
using PeakyFlow.Application.GameMaps.ThrowDice;
using PeakyFlow.Application.Rooms.LeaveRoom;
using PeakyFlow.Application.RoomStates.AcceptCard;
using PeakyFlow.Application.RoomStates.BankruptAction;
using PeakyFlow.Application.RoomStates.Borrow;
using PeakyFlow.Application.RoomStates.GetPlayerState;
using PeakyFlow.Application.RoomStates.IsCardAcceptable;
using PeakyFlow.Application.RoomStates.PullDealCard;
using PeakyFlow.Application.RoomStates.Repair;
using PeakyFlow.GrpcProtocol.Common;
using PeakyFlow.GrpcProtocol.Game;
using PeakyFlow.Server.Common.Extensions;
using PeakyFlow.Server.Common.Interfaces;
using System.Reactive.Linq;

namespace PeakyFlow.Server.Services
{
    public class GameGrpcService(
        INotificationReceiver<PlayerStartedTurnEvent> playerStartedTurnReceiver,
        INotificationReceiver<PlayerStateCreatedEvent> playerStateCreatedReceiver,
        INotificationReceiver<AnotherPlayerStateChangedEvent> anotherPlayerStateChangedReceiver,
        INotificationReceiver<PlayerLostEvent> playerLostEventReceiver,
        IMediator mediator,
        IMapper mapper) : GameRpcService.GameRpcServiceBase
    {
        public override async Task<GetPlayerStateResp> GetPlayerState(PlayerSubscriptionMessage request, ServerCallContext context)
        {
            var result = await mediator.Send(new GetPlayerStateQuery(request.RoomId, request.PlayerId), context.CancellationToken);
            var resp = new GetPlayerStateResp()
            {
                BaseResp = result.ToRespBase(mapper),
                Player = mapper.Map<PlayerStateMsg>(result.Value)
            };

            return resp;
        }

        public override async Task<GetGameMapResp> GetGameMap(GetGameMapMessage request, ServerCallContext context)
        {
            var result = await mediator.Send(new GetGameMapQuery(request.Id), context.CancellationToken);
            var resp = new GetGameMapResp()
            {
                BaseResp = result.ToRespBase(mapper),
                GameMap = mapper.Map<GameMapResp>(result.Value)
            };

            return resp;
        }

        public override async Task<BorrowResp> Borrow(BorrowMsg request, ServerCallContext context)
        {
            var result = await mediator.Send(new BorrowCommand(request.RoomId, request.PlayerId, request.Money), context.CancellationToken);

            var resp = new BorrowResp()
            {
                BaseResp = result.ToRespBase(mapper),
                PlayerState = mapper.Map<PlayerStateMsg>(result.Value)
            };

            return resp;
        }

        public override async Task<RepairResp> Repair(RepairMsg request, ServerCallContext context)
        {
            var result = await mediator.Send(new RepairCommand(request.RoomId, request.PlayerId, request.LiabilityNames, request.Money));

            var resp = new RepairResp()
            {
                BaseResp = result.ToRespBase(mapper),
                PlayerState = mapper.Map<PlayerStateMsg>(result.Value)
            };

            return resp;
        }

        public override async Task<ThrowDiceResp> ThrowDice(ThrowDiceMsg request, ServerCallContext context)
        {
            var result = await mediator.Send(new ThrowDiceCommand(request.RoomId, request.PlayerId, request.Dice), context.CancellationToken);
            var resp = mapper.Map<ThrowDiceResp>(result.Value) ?? new ThrowDiceResp();

            resp.BaseResp = result.ToRespBase(mapper);

            return resp;
        }

        public override async Task<PullDealCardResp> PullDealCard(PullDealCardMsg request, ServerCallContext context)
        {
            var cardType = mapper.Map<CardType>(request.CardType);
            var result = await mediator.Send(new PullDealCardCommand(request.RoomId, request.PlayerId, cardType), context.CancellationToken);

            var resp = new PullDealCardResp()
            {
                BaseResp = result.ToRespBase(mapper),
                Card = mapper.Map<CardMsg>(result.Value)
            };

            return resp;
        }

        public override async Task<IsCardAcceptableResp> IsCardAcceptable(IsCardAcceptableMsg request, ServerCallContext context)
        {
            var propositions = mapper.Map<IEnumerable<Proposition>>(request.Propositions);
            var result = await mediator.Send(new IsCardAcceptableQuery(request.RoomId, request.PlayerId, request.CardId, request.Count, propositions), context.CancellationToken);
            var resp = mapper.Map<IsCardAcceptableResp>(result.Value) ?? new IsCardAcceptableResp();

            resp.BaseResp = result.ToRespBase(mapper);

            return resp;
        }

        public override async Task<AcceptCardResp> AcceptCard(AcceptCardMsg request, ServerCallContext context)
        {
            var propositions = mapper.Map<IEnumerable<Proposition>>(request.Propositions);

            var result = await mediator.Send(new AcceptCardCommand(request.RoomId, request.PlayerId, request.CardId, request.Count, request.FinancialItemIds, propositions),
                context.CancellationToken);

            var resp = mapper.Map<AcceptCardResp>(result.Value) ?? new AcceptCardResp();

            resp.BaseResp = result.ToRespBase(mapper);

            return resp;
        }

        public override async Task<RespBase> EndTurn(EndTurnMsg request, ServerCallContext context)
        {
            var result = await mediator.Send(new EndTurnCommand(request.RoomId, request.PlayerId), context.CancellationToken);

            var resp = result.ToRespBase(mapper);

            return resp;
        }

        public override async Task<RespBase> LeaveRoom(LeaveRoomMsg request, ServerCallContext context)
        {
            var command = mapper.Map<LeaveRoomCommand>(request);
            var result = await mediator.Send(command, context.CancellationToken);
            var resp = result.ToRespBase(mapper);
            return resp;
        }

        public override async Task<BankruptActionResp> BankruptAction(BankruptActionMsg request, ServerCallContext context)
        {
            var command = mapper.Map<BankruptActionCommand>(request);
            var result = await mediator.Send(command, context.CancellationToken);
            var resp = new BankruptActionResp()
            {
                BaseResp = result.ToRespBase(mapper),
                Player = mapper.Map<PlayerStateMsg>(result.Value)
            };
            return resp;
        }

        public override async Task OnPlayerStartTurn(PlayerSubscriptionMessage request, IServerStreamWriter<PlayerStartTurnResp> responseStream, ServerCallContext context)
        {
            var events = playerStartedTurnReceiver.ReceiveNotifications()
                .Where(x => x.RoomId == request.RoomId)
                .ToAsyncEnumerable();

            await foreach (var startTurn in events.WithCancellation(context.CancellationToken))
            {
                await responseStream.WriteAsync(new PlayerStartTurnResp()
                {
                    PlayerId = startTurn.PlayerId
                }, context.CancellationToken);
            }
        }

        public override async Task OnPlayerCreated(PlayerSubscriptionMessage request, IServerStreamWriter<PlayerCreatedEventMsg> responseStream, ServerCallContext context)
        {
            var events = playerStateCreatedReceiver.ReceiveNotifications()
                .Where(x => x.RoomId == request.RoomId)
                .Where(x => x.Id == request.PlayerId)
                .Select(mapper.Map<PlayerCreatedEventMsg>)
                .ToAsyncEnumerable();

            await foreach (var e in events)
            {
                await responseStream.WriteAsync(e, context.CancellationToken);
            }
        }

        public override async Task OnPlayerLeftRoom(PlayerSubscriptionMessage request, IServerStreamWriter<PlayerLeftRoomMsg> responseStream, ServerCallContext context)
        {
            var events = playerLostEventReceiver.ReceiveNotifications()
                .Where(x => x.RoomId == request.RoomId)
                .Where(x => x.PlayerId != request.PlayerId)
                .ToAsyncEnumerable();

            await foreach (var e in events)
            {
                await responseStream.WriteAsync(new PlayerLeftRoomMsg()
                {
                    PlayerId = e.PlayerId,
                    RoomId = e.RoomId,
                }, context.CancellationToken);
            }
        }

        public override async Task OnAnotherPlayerStateChanged(PlayerSubscriptionMessage request, IServerStreamWriter<AnotherPlayerStateChangedMsg> responseStream, ServerCallContext context)
        {
            var events = anotherPlayerStateChangedReceiver.ReceiveNotifications()
                .Where(x => x.RoomStateId == request.RoomId)
                .Where(x => x.PlayerId != request.PlayerId)
                .Select(mapper.Map<AnotherPlayerStateChangedMsg>)
                .ToAsyncEnumerable();

            await foreach (var e in events)
            {
                await responseStream.WriteAsync(e, context.CancellationToken);
            }
        }
    }
}
