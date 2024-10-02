using Ardalis.Result;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.LobbyGame.GetLobby
{
    public class GetLobbyHandler : LobbyContextHandlerBase<GetLobbyQuery, Result<LobbyDto>>
    {
        public GetLobbyHandler(ILogger<GetLobbyHandler> logger, IRepository<Lobby> repository) : base(logger, repository)
        {
        }

        protected override Result<LobbyDto> NotFoundResponse => Result<LobbyDto>.NotFound();



        protected override Task<Result<LobbyDto>> Handle(GetLobbyQuery request, Lobby l, CancellationToken cancellationToken)
        {
            if (!l.Players.Any(x => x.Id == request.PlayerId))
            {
                return Task.FromResult(Result<LobbyDto>.Unauthorized());
            }

            return Task.FromResult(Result<LobbyDto>.Success(new LobbyDto(
                l.Id, 
                l.Owner, 
                l.Name, 
                l.Password, 
                l.Created, 
                l.PlayersNumber,
                l.IsFree, 
                l.IsPublic, 
                l.TeamSize, 
                l.IsClosed, 
                l.Players.Select(x => 
                new LobbyPlayerDto(
                    x.Id, 
                    x.Name,
                    x.LobbyId,
                    x.IsReady, 
                    x.IsOwner)))));
        }
    }
}
