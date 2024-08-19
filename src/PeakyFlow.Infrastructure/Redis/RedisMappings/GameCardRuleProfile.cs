using AutoMapper;
using PeakyFlow.Abstractions.GameCardRuleAggregate;
using PeakyFlow.Infrastructure.Redis.Models;

namespace PeakyFlow.Infrastructure.Redis.RedisMappings
{
    public class GameCardRuleProfile : Profile
    {
        public GameCardRuleProfile()
        {
            CreateMap<GameCardRule, GameCardRuleM>().ReverseMap();
        }
    }
}
