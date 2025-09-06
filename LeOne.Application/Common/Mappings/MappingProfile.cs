using AutoMapper;
using LeOne.Application.SpaServices.Dtos;
using LeOne.Domain.Entities;

namespace LeOne.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // SpaService
            CreateMap<SpaService, SpaServiceDto>();
        }
    }
}
