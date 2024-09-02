using MediatR;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Specifications;

namespace PeakyFlow.Application.Rooms.Handlers
{
    public class LobbyClosedAndGameStartedEventForRoomHandler : INotificationHandler<LobbyClosedAndGameStartedEvent>
    {
        private readonly IRepository<Room> _roomRepository;

        public LobbyClosedAndGameStartedEventForRoomHandler(IRepository<Room> roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task Handle(LobbyClosedAndGameStartedEvent notification, CancellationToken cancellationToken)
        {
            var existingRoom = await _roomRepository.FirstOrDefaultAsync(new FirstOrDefaultByIdSpecification<Room>(notification.LobbyId), cancellationToken);

            if (existingRoom != null)
            {
                return;
            }

            var room = new Room()
            {
                Id = notification.LobbyId,
                Name = notification.Name,
                Players = notification.Players.Select(p => new PlayerInRoom()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = PlayerInRoomStatus.Active
                })
            };

            await _roomRepository.AddAsync(room, cancellationToken);
        }
    }
}
