using api_aspnetcore6.Dtos.Category;
using api_aspnetcore6.Dtos.Order;
using api_aspnetcore6.Dtos.OrderDetail;
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
            CreateMap<Category, CategoryRequest>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();

            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, ProductRequest>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();

            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<Order, OrderRequest>().ReverseMap();
            CreateMap<Order, OrderResponse>().ReverseMap();

            CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailReponse>().ReverseMap();
        }
    }
}