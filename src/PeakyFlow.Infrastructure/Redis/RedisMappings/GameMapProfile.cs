using AutoMapper;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Infrastructure.Redis.Models;

namespace PeakyFlow.Infrastructure.Redis.RedisMappings
{
    public class GameMapProfile : Profile
    {
        public GameMapProfile()
        {
            CreateMap<GameMap, GameMapM>().ReverseMap();
            CreateMap<GameMapPlayer, GameMapPlayerM>().ReverseMap();
        }
    }
}
