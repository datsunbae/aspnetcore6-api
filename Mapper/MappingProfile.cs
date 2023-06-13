using api_aspnetcore6.Dtos.Category;
using api_aspnetcore6.Dtos.Product;
using api_aspnetcore6.Models;
using AutoMapper;

namespace api_aspnetcore6.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();
        }
    }
}