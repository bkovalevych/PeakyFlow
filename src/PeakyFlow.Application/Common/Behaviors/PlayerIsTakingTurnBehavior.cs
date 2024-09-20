using Ardalis.Result;
using MediatR;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Application.Common.Interfaces;
using System.Reflection;

namespace PeakyFlow.Application.Common.Behaviors
{
    public class PlayerIsTakingTurnBehavior<TRequest, TResponse>(IRepository<GameMap> gameMapRepository)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IPlayerIsTakingTurnRequest
    {
        public const string Msg = "It is not your turn";
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var gameMap = await gameMapRepository.GetByIdAsync(request.RoomId, cancellationToken);

            if (gameMap == null || gameMap.TakingTurnPlayer == null || gameMap.TakingTurnPlayer == request.PlayerId)
            {
                return await next();
            }

            var resultGenericType = typeof(TResponse).GetGenericArguments().FirstOrDefault();
            var isResult = typeof(TResponse) == typeof(Result);
            var isGenericResult = resultGenericType != null && typeof(TResponse) == typeof(Result<>).MakeGenericType(resultGenericType);


            if (isResult && (TResponse?)(object?)Result.Conflict(Msg) is TResponse castedResponse)
            {
                return castedResponse;
            }

            if (resultGenericType == null || !isGenericResult)
            {
                return await next();
            }

            var resultTypeDefinition = typeof(Result<>).MakeGenericType(resultGenericType);
            var method = resultTypeDefinition
                        .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                        .FirstOrDefault(m => m.Name == "Conflict" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(string[]));
            
            if (method == null)
            {
                return await next();
            }

            var result = (TResponse?)(object?)method.Invoke(null, [new string[] { Msg }]);
            if (result == null)
            {
                return await next();
            }

            return result;
        }
    }
}
