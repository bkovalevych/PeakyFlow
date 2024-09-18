using Grpc.Core;
using Grpc.Core.Interceptors;
using MediatR;
using PeakyFlow.Abstractions.RoomAggregate.Events;
using PeakyFlow.GrpcProtocol.Game;

namespace PeakyFlow.Server.Interceptors
{
    public class ExceptionHanlerInterceptor(
        IMediator mediator,
        ILogger<ExceptionHanlerInterceptor> logger) : Interceptor
    {
        

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception caught: {ex.Message}");

                throw new RpcException(new Status(StatusCode.Internal, "An internal server error occurred."));
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                await continuation(request, responseStream, context);
            }
            catch (TaskCanceledException) when (request is PlayerSubscriptionMessage subscription)
            {
                logger.LogWarning($"Player {subscription.PlayerId} from room {subscription.RoomId} has lost connection");
                await mediator.Publish(new PlayerLostEvent(subscription.RoomId, subscription.PlayerId));
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception caught: {ex.Message}");

                throw new RpcException(new Status(StatusCode.Internal, "An internal server error occurred."));
            }
        }
    }
}
