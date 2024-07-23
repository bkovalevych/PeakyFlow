using MediatR;
using Microsoft.Extensions.Logging;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Roles.GetRoleForPlayer;

namespace PeakyFlow.Application.RoomStates.Handlers
{
    public class LobbyClosedAndGameStartedEventForRoomStateHandler : INotificationHandler<LobbyClosedAndGameStartedEvent>
    {
        private readonly IMediator _mediator;
        private readonly IRepository<RoomState> _roomStateRepository;
        private readonly IGetRoleForPlayerService _getRoleForPlayerService;
        private readonly ILogger<LobbyClosedAndGameStartedEventForRoomStateHandler> _logger;

        public LobbyClosedAndGameStartedEventForRoomStateHandler(
            IMediator mediator,
            IGetRoleForPlayerService getRoleForPlayerService,
            IRepository<RoomState> roomStateRepository,
            ILogger<LobbyClosedAndGameStartedEventForRoomStateHandler> logger)
        {
            _mediator = mediator;
            _roomStateRepository = roomStateRepository;
            _getRoleForPlayerService = getRoleForPlayerService;
            _logger = logger;
        }

        public async Task Handle(LobbyClosedAndGameStartedEvent notification, CancellationToken cancellationToken)
        {
            var state = new RoomState()
            {
                Id = notification.LobbyId
            };

            var players = new List<PlayerState>();

            foreach (var p in notification.players)
            {
                var role = await _getRoleForPlayerService.GetRoleForPlayer(cancellationToken);
                if (!role.IsSuccess)
                {
                    _logger.LogWarning("Unsuccesful getting role");
                    return;
                }

                var playerState = new PlayerState()
                {
                    Id = p.Id,
                    Description = role.Value.Description,
                    Name = p.Name,
                    RoleName = role.Value.RoleName,
                    CountableLiabilities = role.Value.CountableLiabilities,
                    FinancialItems = role.Value.FinancialItems,
                    ImageId = role.Value.ImageId,
                    PercentableLiabilities = role.Value.PercentableLiabilities,
                    Savings = role.Value.InitialSavings,
                    Stocks = role.Value.Stocks
                };

                players.Add(playerState);
            }

            state.PlayerStates = players;

            await _roomStateRepository.AddAsync(state, cancellationToken);
            await _roomStateRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("State for room {room} inited", state.Id);

            foreach (var player in players)
            {
                var playerCreatedEvent = new PlayerStateCreatedEvent(
                    player.Id,
                    player.Name,
                    state.Id,
                    player.Savings,
                    player.IsBankrupt,
                    player.RoleName,
                    player.Description,
                    player.ImageId,
                    player.CountableLiabilities,
                    player.PercentableLiabilities,
                    player.Stocks,
                    player.FinancialItems,
                    player.Salary,
                    player.Expenses,
                    player.Income,
                    player.CashFlow,
                    player.ExpensesForOneChild);

                await _mediator.Publish(playerCreatedEvent, cancellationToken);
            }
        }
    }
}