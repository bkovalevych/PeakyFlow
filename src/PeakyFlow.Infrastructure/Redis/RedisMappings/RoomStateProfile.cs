using AutoMapper;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Infrastructure.Redis.Models;

namespace PeakyFlow.Infrastructure.Redis.RedisMappings
{
    public class RoomStateProfile : Profile
    {
        public RoomStateProfile()
        {
            CreateMap<RoomState, RoomStateM>().ReverseMap();

            CreateMap<PlayerState, PlayerStateM>().ReverseMap();
        }
    }
}
