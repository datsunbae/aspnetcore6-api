using api_aspnetcore6.Dtos;
using api_aspnetcore6.Models;
using AutoMapper;

namespace api_aspnetcore6.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
        }
    }
}