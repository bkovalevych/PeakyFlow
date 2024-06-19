using AutoMapper;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.LobbyGame.List;

namespace PeakyFlow.Application.Common.Mappings
{
    public class LobbyMapperProfies : Profile
    {
        public LobbyMapperProfies()
        {
            CreateMap<Lobby, LobbyListResponse>()
                .ForMember(x => x.Id, x => x.MapFrom(y => y.LobbyInfo.Id))
                .ForMember(x => x.Name, x => x.MapFrom(y => y.LobbyInfo.Name))
                .ForMember(x => x.Owner, x => x.MapFrom(y => y.LobbyInfo.Owner))
                .ForMember(x => x.Created, x => x.MapFrom(y => y.LobbyInfo.Created));
        }
    }
}
