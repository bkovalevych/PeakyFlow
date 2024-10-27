using Ardalis.Result;
using AutoMapper;
using MediatR;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.List
{
    public class LobbyListHandler(IRepository<Lobby> _repository, IMapper _mapper) : IRequestHandler<LobbyListQuery, Result<IEnumerable<LobbyListResponse>>>
    {
        public async Task<Result<IEnumerable<LobbyListResponse>>> Handle(LobbyListQuery request, CancellationToken cancellationToken)
        {
            await _repository.Init();
            if (request.PaginationCount == 0)
            {
                request.PaginationCount = 20;
            }
            var entities = await _repository.ListAsync(new LobbyListSpecification(request), cancellationToken);
            var mapped = _mapper.Map<IEnumerable<LobbyListResponse>>(entities);


            return Result<IEnumerable<LobbyListResponse>>.Success(mapped);
        }
    }
}
