using Ardalis.Result;
using PeakyFlow.Abstractions.GameRoleAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.Roles.GetRoleForPlayer
{
    public class GetRoleForPlayerService : IGetRoleForPlayerService
    {
        private readonly IRepository<GameRole> _gameRoleRepository;

        public GetRoleForPlayerService(IRepository<GameRole> gameRoleRepository)
        {
            _gameRoleRepository = gameRoleRepository;
        }

        public async Task<Result<GameRole>> GetRoleForPlayer(CancellationToken cancellationToken)
        {
            var count = await _gameRoleRepository.CountAsync(cancellationToken);

            var random = new Random();

            if (count == 0)
            {
                throw new ArgumentException("Game has no roles");
            }

            var index = random.Next(count - 1);

            var role = await _gameRoleRepository.FirstOrDefaultAsync(new GetRoleForPlayerSpecification(index));

            if (role == null)
            {
                return Result<GameRole>.NotFound();
            }

            return role;
        }
    }
}
