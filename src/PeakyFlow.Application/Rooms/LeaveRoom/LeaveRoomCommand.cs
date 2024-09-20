using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.RoomAggregate;

namespace PeakyFlow.Application.Rooms.LeaveRoom
{
    public record LeaveRoomCommand(string RoomId, string PlayerId, PlayerInRoomStatus Status) 
        : IRequest<Result>;
}
