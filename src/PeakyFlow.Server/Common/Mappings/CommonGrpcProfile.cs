using Ardalis.Result;
using AutoMapper;
using PeakyFlow.GrpcProtocol.Common;

namespace PeakyFlow.Server.Common.Mappings
{
    public class CommonGrpcProfile : Profile
    {
        public CommonGrpcProfile()
        {
            CreateMap<ValidationError, ValidationErrorMsg>();
            CreateMap<ResultStatus, ResultStatusMsg>();
        }
    }
}
