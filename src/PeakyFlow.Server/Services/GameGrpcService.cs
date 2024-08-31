using AutoMapper;
using Grpc.Core;
using MediatR;
using PeakyFlow.Application.RoomStates.GetPlayerState;
using PeakyFlow.GrpcProtocol.Game;
using PeakyFlow.Server.Common.Extensions;

namespace PeakyFlow.Server.Services
{
    public class GameGrpcService(
        IMediator mediator,
        IMapper mapper) : GameRpcService.GameRpcServiceBase
    {
        public override async Task<GetplayerStateResp> GetplayerState(PlayerSubscriptionMessage request, ServerCallContext context)
        {
            var result = await mediator.Send(new GetPlayerStateQuery(request.RoomId, request.PlayerId), context.CancellationToken);
            var resp = new GetplayerStateResp()
            {
                BaseResp = result.ToRespBase(mapper),
                Player = mapper.Map<PlayerStateMsg>(result.Value)
            };

            return resp;
        }
    }
}
