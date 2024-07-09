using AutoMapper;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.LobbyGame.List;

namespace PeakyFlow.Application.Common.Mappings
{
    public class LobbyMapperProfies : Profile
    {
        public LobbyMapperProfies()
        {
            CreateMap<Lobby, LobbyListResponse>();
        }
    }
}
