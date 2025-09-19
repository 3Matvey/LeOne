using AutoMapper;
using LeOne.Application.Products.Dtos;
using LeOne.Application.SpaServices.Dtos;
using LeOne.Application.Reviews.Dtos;
using LeOne.Domain.Entities;

namespace LeOne.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // SpaService
            CreateMap<SpaService, SpaServiceDto>();

            // Product
            CreateMap<Product, ProductDto>();

            // Review
            CreateMap<Review, ReviewDto>();
        }
    }
}
