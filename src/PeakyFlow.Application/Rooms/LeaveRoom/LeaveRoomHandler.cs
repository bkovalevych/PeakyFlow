using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Application.Rooms.LeaveRoom
{
    public class LeaveRoomHandler : IRequestHandler<LeaveRoomCommand, Result>
    {
        private readonly IRepository<Room> _roomRep;

        public LeaveRoomHandler(IRepository<Room> roomRep)
        {
            _roomRep = roomRep;
        }

        public async Task<Result> Handle(LeaveRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await _roomRep.GetByIdAsync(request.RoomId, cancellationToken);

            if (room == null)
            {
                return Result.NotFound();
            }

            var player = room.SetStatus(request.PlayerId, request.Status);

            if (player == null) 
            {
                return Result.NotFound();
            }

            return Result.Success();
        }
    }
}
