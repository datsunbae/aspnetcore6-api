using api_aspnetcore6.Dtos;
using api_aspnetcore6.Models;

namespace api_aspnetcore6.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts(PagingParameters pagingParameters);
        Task<List<Product>> SearchProduct(string? name, double? priceFrom, double? priceTo, int? idCategory, PagingParameters pagingParameters);
        Task<Product> GetProductById(int id);
        Task<Product> AddProduct(Product product);
        Task<Product> UpdateProduct(int id, Product product);
        Task<int> DeleteProduct(int id);
    }
}