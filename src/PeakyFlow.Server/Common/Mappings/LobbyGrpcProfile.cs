using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.LobbyGame.CloseLobbyAndStartGame;
using PeakyFlow.Application.LobbyGame.JoinLobby;
using PeakyFlow.Application.LobbyGame.List;
using PeakyFlow.GrpcProtocol.Lobby;

namespace PeakyFlow.Server.Common.Mappings
{
    public class LobbyGrpcProfile : Profile
    {
        public LobbyGrpcProfile()
        {
            CreateMap<LobbyCreatedEvent, LobbyItem>()
                .ForMember(x => x.Created, x => x.MapFrom(x => Timestamp.FromDateTimeOffset(x.Created)));

            CreateMap<LobbyItem, LobbyCreatedEvent>()
                .ForMember(x => x.Created, x => x.MapFrom(x => x.Created.ToDateTimeOffset()));
            CreateMap<PlayerInLobby, PlayerMessage>().ReverseMap();

            CreateMap<CloseLobbyAndStartGameResponse, CloseLobbyAndStartGameResp>()
                .ReverseMap();

            CreateMap<JoinLobbyResponse, JoinLobbyResp>().ReverseMap();
            CreateMap<LobbyListResponse, LobbyListResp>().ReverseMap();

            CreateMap<LobbyListResponse, LobbyItem>().ReverseMap();

            CreateMap<LobbyClosedAndGameStartedEvent, LobbyClosedAndGameStartedMessage>()
                .ReverseMap();

            CreateMap<PlayerBase, PlayerMessage>().ReverseMap();

            CreateMap<PlayerJoinedEvent, PlayerJoinedToLobbyMessage>();

        }
    }
}
