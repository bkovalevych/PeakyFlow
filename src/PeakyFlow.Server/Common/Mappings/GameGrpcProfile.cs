using AutoMapper;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.GameRoleAggregate;
using PeakyFlow.Application.RoomStates;
using PeakyFlow.GrpcProtocol.Game;
using static PeakyFlow.GrpcProtocol.Game.GameMapResp.Types;

namespace PeakyFlow.Server.Common.Mappings
{
    public class GameGrpcProfile : Profile
    {
        public GameGrpcProfile()
        {
            CreateMap<PlayerStateDto, PlayerStateMsg>().ReverseMap();
            CreateMap<FinancialItemBase, FinancialItemMsg>().ReverseMap();
            CreateMap<FinancialType, FinancialTypeMsg>().ReverseMap();

            CreateMap<GameMap, GameMapResp>().ReverseMap();
            CreateMap<GameMapPlayer, GameMapPlayerResp>().ReverseMap();
            CreateMap<StepType, StepTypeMsg>();
        }
    }
}
