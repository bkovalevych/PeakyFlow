using AutoMapper;
using Grpc.Core;
using MediatR;
using PeakyFlow.Application.LobbyGame.Create;
using PeakyFlow.GrpcProtocol.Common;
using PeakyFlow.GrpcProtocol.Lobby;
using static System.Net.WebRequestMethods;

namespace PeakyFlow.Server.Services
{
    public class LobbyGrpcService(IMediator mediator, IMapper mapper) : LobbyRpcService.LobbyRpcServiceBase
    {
        public override async Task<CreateLobbyResp> CreateLobby(CreateLobbyMessage request, ServerCallContext context)
        {
            var command = new CreateLobbyCommand(request.Owner, request.Name, request.TeamSize, request.Password);

            var result = await mediator.Send(command, context.CancellationToken);

            var resp = new CreateLobbyResp();

            resp.BaseResp = new RespBase()
            {
                Errors = { result.Errors },
                ValidationErrors = { mapper.Map<List<ValidationErrorMsg>>(result.ValidationErrors) },
                Status = mapper.Map<ResultStatusMsg>(result.Status)
            };
            resp.Id = result.Value;

            return resp;
        }
    }
}
