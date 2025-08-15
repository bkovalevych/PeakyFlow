using AutoMapper;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Infrastructure.AzureTable.Models;

namespace PeakyFlow.Infrastructure.AzureTable.Mappings
{
    public class AzureTableMappingProfile : Profile
    {
        public AzureTableMappingProfile()
        {
            CreateMap<LobbyTableEntity, Lobby>()
                .ForMember(x => x.Players, x => x.Ignore())
                .ForMember(x => x.Id, x => x.MapFrom(x => x.RowKey))
                .ForSourceMember(x => x.PlayersSerialized, x => x.DoNotValidate())
                .ReverseMap()
                .ForMember(x => x.PlayersSerialized, x => x.Ignore())
                .ForMember(x => x.RowKey, x => x.MapFrom(x => x.Id));

            CreateMap<RoomTableEntity, Room>()
                .ForMember(x => x.Id, x => x.MapFrom(x => x.RowKey))
                .ForMember(x => x.Players, x => x.Ignore())
                .ReverseMap()
                .ForMember(x => x.RowKey, x => x.MapFrom(x => x.Id))
                .ForMember(x => x.PlayersRaw, x => x.Ignore());
            
            CreateMap<RoomStateTableEntity, RoomState>()
                .ForMember(x => x.Id, x => x.MapFrom(x => x.RowKey))
                .ForMember(x => x.Cards, x => x.Ignore())
                .ForMember(x => x.Indeces, x => x.Ignore())
                .ForMember(x => x.PlayerStates, x => x.Ignore())
                .ReverseMap()
                .ForMember(x => x.RowKey, x => x.MapFrom(x => x.Id))
                .ForMember(x => x.CardsRaw, x => x.Ignore())
                .ForMember(x => x.IndecesRaw, x => x.Ignore())
                .ForMember(x => x.PlayerStatesRaw, x => x.Ignore());

            CreateMap<GameMapTableEntity, GameMap>()
                .ForMember(x => x.Id, x => x.MapFrom(x => x.RowKey))
                .ForMember(x => x.GameMapPlayers, x => x.Ignore())
                .ForMember(x => x.Steps, x => x.Ignore())
                .ReverseMap()
                .ForMember(x => x.RowKey, x => x.MapFrom(x => x.Id))
                .ForMember(x => x.GameMapPlayersRaw, x => x.Ignore())
                .ForMember(x => x.StepsRaw, x => x.Ignore());
        }
    }
}
