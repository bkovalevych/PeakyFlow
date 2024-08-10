using Ardalis.Result;
using MediatR;

namespace PeakyFlow.Application.RoomStates.IsCardAcceptable
{
    public record IsCardAcceptableQuery(string RoomId, string PlayerId, string CardId, int? Count) : IRequest<Result<IsCardAcceptableResponse>>;
}
