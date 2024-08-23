using AutoMapper;
using PeakyFlow.Abstractions.GameRoleAggregate;
using PeakyFlow.Infrastructure.Redis.Models;

namespace PeakyFlow.Infrastructure.Redis.RedisMappings
{
    public class GameRoleProfile : Profile
    {
        public GameRoleProfile()
        {
            CreateMap<GameRole, GameRoleM>().ReverseMap();
        }
    }
}
