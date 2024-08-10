using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.GameCardRuleAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.RoomStates.IsCardAcceptable
{
    public class IsCardAcceptableHandler : IRequestHandler<IsCardAcceptableQuery, Result<IsCardAcceptableResponse>>
    {
        private readonly IRepository<RoomState> _roomStateRepository;
        private readonly IReadRepository<GameCardRule> _gameCardRuleRepository;

        public IsCardAcceptableHandler(IRepository<RoomState> roomStateRepository,
            IReadRepository<GameCardRule> gameCardRuleRepository)
        {
            _roomStateRepository = roomStateRepository;
            _gameCardRuleRepository = gameCardRuleRepository;
        }

        public async Task<Result<IsCardAcceptableResponse>> Handle(IsCardAcceptableQuery request, CancellationToken cancellationToken)
        {
            var cardRule = await _gameCardRuleRepository.FirstOrDefaultAsync(new FirstOrDefaultCardRuleSpecification(), cancellationToken);

            if (cardRule == null)
            {
                return Result<IsCardAcceptableResponse>.NotFound();
            }

            var room = await _roomStateRepository.FirstOrDefaultAsync(new FirstOrDefaultByIdSpecification<RoomState>(request.RoomId), cancellationToken);

            if (room == null)
            {
                return Result<IsCardAcceptableResponse>.NotFound();
            }

            var card = cardRule.Cards.First(x => x.Id == request.CardId);

            var p = room.IsCardAcceptable(card, request.PlayerId, request.Count);

            if (!p.Successfuly)
            {
                return Result<IsCardAcceptableResponse>.NotFound();
            }

            return new IsCardAcceptableResponse(p.Acceptable, p.HowMuchToBorrow);
        }
    }
}
