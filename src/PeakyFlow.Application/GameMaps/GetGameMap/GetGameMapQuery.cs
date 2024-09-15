using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.GameMapAggregate;

namespace PeakyFlow.Application.GameMaps.GetGameMap
{
    public record GetGameMapQuery(string RoomId) : IRequest<Result<GameMap>>;
}
