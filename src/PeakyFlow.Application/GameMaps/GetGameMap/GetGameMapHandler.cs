using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.GameMaps.GetGameMap
{
    public class GetGameMapHandler(IRepository<GameMap> rep) : IRequestHandler<GetGameMapQuery, Result<GameMap>>
    {
        public async Task<Result<GameMap>> Handle(GetGameMapQuery request, CancellationToken cancellationToken)
        {
            await rep.Init();
            var result = await rep.GetByIdAsync(request.RoomId, cancellationToken);

            if (result == null) 
            {
                return Result<GameMap>.NotFound();
            }

            return result;
        }
    }
}
