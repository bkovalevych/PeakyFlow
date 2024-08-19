using AutoMapper;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Infrastructure.Redis.Models;

namespace PeakyFlow.Infrastructure.Redis.RedisMappings
{
    public class LobbyProfile : Profile
    {
        public LobbyProfile()
        {
            CreateMap<Lobby, LobbyM>()
                .ReverseMap();
            CreateMap<PlayerInLobby, PlayerInLobbyM>()
                .ReverseMap();
        }
    }
}
