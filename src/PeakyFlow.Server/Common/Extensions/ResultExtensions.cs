using Ardalis.Result;
using AutoMapper;
using PeakyFlow.GrpcProtocol.Common;

namespace PeakyFlow.Server.Common.Extensions
{
    public static class ResultExtensions
    {
        public static RespBase ToRespBase<T>(this Result<T> instance, IMapper mapper)
        {
            return new RespBase()
            {
                Errors = { instance.Errors },
                ValidationErrors = { mapper.Map<List<ValidationErrorMsg>>(instance.ValidationErrors) },
                Status = mapper.Map<ResultStatusMsg>(instance.Status)
            };
        }

        public static RespBase ToRespBase(this Result instance, IMapper mapper)
        {
            return new RespBase()
            {
                Errors = { instance.Errors },
                ValidationErrors = { mapper.Map<List<ValidationErrorMsg>>(instance.ValidationErrors) },
                Status = mapper.Map<ResultStatusMsg>(instance.Status)
            };
        }
    }
}
