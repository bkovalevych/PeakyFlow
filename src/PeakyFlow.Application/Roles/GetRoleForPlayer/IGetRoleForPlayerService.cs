using Ardalis.Result;
using PeakyFlow.Abstractions.GameRoleAggregate;

namespace PeakyFlow.Application.Roles.GetRoleForPlayer
{
    public interface IGetRoleForPlayerService
    {
        Task<Result<GameRole>> GetRoleForPlayer(CancellationToken cancellationToken);
    }
}
