using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;

namespace PeakyFlow.Application.Common.Mappings
{
    public class ValidationProfile : Profile
    {
        public ValidationProfile()
        {
            CreateMap<ValidationFailure, ValidationError>()
                .ForMember(x => x.ErrorMessage, x => x.MapFrom(x => x.ErrorMessage))
                .ForMember(x => x.Identifier, x => x.MapFrom(x => x.PropertyName))
                .ForMember(x => x.ErrorCode, x => x.MapFrom(x => x.ErrorCode))
                .ForMember(x => x.Severity, x => x.MapFrom(x => x.Severity))
                .ReverseMap();

            CreateMap<Severity, ValidationSeverity>().ReverseMap();
        }
    }
}
