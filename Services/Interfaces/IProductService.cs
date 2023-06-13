using api_aspnetcore6.Dtos;
using api_aspnetcore6.Dtos.Product;

namespace api_aspnetcore6.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllProducts(PagingParameters pagingParameters);
        Task<IEnumerable<ProductResponse>> SearchProduct(string? name, double? priceFrom, double? priceTo, int? idCategory, PagingParameters pagingParameters);
        Task<ProductResponse> GetProductById(int id);
        Task<ProductResponse> AddProduct(ProductDTO product);
        Task<ProductResponse> UpdateProduct(int id, ProductRequest product);
        Task<int> DeleteProduct(int id);
    }
}