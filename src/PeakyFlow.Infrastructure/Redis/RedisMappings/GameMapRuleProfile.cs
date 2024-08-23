using AutoMapper;
using PeakyFlow.Abstractions.GameMapRuleAggregate;
using PeakyFlow.Infrastructure.Redis.Models;

namespace PeakyFlow.Infrastructure.Redis.RedisMappings
{
    public class GameMapRuleProfile : Profile
    {
        public GameMapRuleProfile()
        {
            CreateMap<GameMapRule, GameMapRuleM>().ReverseMap();
        }
    }
}
