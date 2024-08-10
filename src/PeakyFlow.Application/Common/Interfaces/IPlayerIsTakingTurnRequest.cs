namespace PeakyFlow.Application.Common.Interfaces
{
    public interface IPlayerIsTakingTurnRequest
    {
        string RoomId { get; }
        string PlayerId { get; }
    }
}
