using AutoMapper;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.GameRoleAggregate;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Application.GameMaps.ThrowDice;
using PeakyFlow.Application.Rooms.LeaveRoom;
using PeakyFlow.Application.RoomStates;
using PeakyFlow.Application.RoomStates.AcceptCard;
using PeakyFlow.Application.RoomStates.BankruptAction;
using PeakyFlow.Application.RoomStates.IsCardAcceptable;
using PeakyFlow.GrpcProtocol.Game;
using static PeakyFlow.GrpcProtocol.Game.CardMsg.Types;
using static PeakyFlow.GrpcProtocol.Game.GameMapResp.Types;
using static PeakyFlow.GrpcProtocol.Game.LeaveRoomMsg.Types;

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

            CreateMap<ThrowDiceResponse, ThrowDiceResp>().ReverseMap();
            CreateMap<Card, CardMsg>().ReverseMap();
            CreateMap<CardType, CardTypeMsg>().ReverseMap();
            CreateMap<StockAction, StockActionMsg>().ReverseMap();

            CreateMap<IsCardAcceptableResponse, IsCardAcceptableResp>().ReverseMap();
            CreateMap<PropositionMsg, Proposition>().ReverseMap();

            CreateMap<AcceptCardResponse, AcceptCardResp>().ReverseMap();

            CreateMap<LeaveRoomMsg, LeaveRoomCommand>().ReverseMap();
            CreateMap<PlayerInRoomStatus, PlayerInRoomStatusMsg>().ReverseMap();

            CreateMap<BankruptActionMsg, BankruptActionCommand>().ReverseMap();
        }
    }
}
