using AutoMapper;
using Grpc.Core;
using MediatR;
using PeakyFlow.Application.GameMaps.GetGameMap;
using PeakyFlow.Application.RoomStates.Borrow;
using PeakyFlow.Application.RoomStates.GetPlayerState;
using PeakyFlow.Application.RoomStates.Repair;
using PeakyFlow.GrpcProtocol.Game;
using PeakyFlow.Server.Common.Extensions;

namespace PeakyFlow.Server.Services
{
    public class GameGrpcService(
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
    }
}
