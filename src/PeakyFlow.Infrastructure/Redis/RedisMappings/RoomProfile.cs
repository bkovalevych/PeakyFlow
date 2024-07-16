using AutoMapper;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Infrastructure.Redis.Models;

namespace PeakyFlow.Infrastructure.Redis.RedisMappings
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<RoomM, Room>()
                .ReverseMap();
            CreateMap<PlayerInRoomM, PlayerInRoom>()
                .ReverseMap();
        }
    }
}
