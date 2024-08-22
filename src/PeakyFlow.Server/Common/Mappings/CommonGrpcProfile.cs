using Ardalis.Result;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using PeakyFlow.GrpcProtocol.Common;

namespace PeakyFlow.Server.Common.Mappings
{
    public class CommonGrpcProfile : Profile
    {
        public CommonGrpcProfile()
        {
            CreateMap<ValidationError, ValidationErrorMsg>();
            CreateMap<ResultStatus, ResultStatusMsg>();

            CreateMap<DateTimeOffset, Timestamp>()
                .ConvertUsing(dto => Timestamp.FromDateTimeOffset(dto));

            CreateMap<Timestamp, DateTimeOffset>()
                .ConvertUsing(ts => ts.ToDateTimeOffset());
        }
    }
}
