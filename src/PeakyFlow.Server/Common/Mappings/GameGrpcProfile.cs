using AutoMapper;
using PeakyFlow.Abstractions.GameRoleAggregate;
using PeakyFlow.Application.RoomStates;
using PeakyFlow.GrpcProtocol.Game;

namespace PeakyFlow.Server.Common.Mappings
{
    public class GameGrpcProfile : Profile
    {
        public GameGrpcProfile()
        {
            CreateMap<PlayerStateDto, PlayerStateMsg>().ReverseMap();
            CreateMap<FinancialItemBase, FinancialItemMsg>().ReverseMap();
            CreateMap<FinancialType, FinancialTypeMsg>().ReverseMap();
        }
    }
}
