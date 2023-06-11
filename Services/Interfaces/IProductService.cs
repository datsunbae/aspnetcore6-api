using api_aspnetcore6.Dtos;

namespace api_aspnetcore6.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllProducts(PagingParameters pagingParameters);
        Task<List<ProductDTO>> SearchProduct(string? name, double? priceFrom, double? priceTo, int? idCategory, PagingParameters pagingParameters);
        Task<ProductDTO> GetProductById(int id);
        Task<ProductDTO> AddProduct(ProductDTO product);
        Task<ProductDTO> UpdateProduct(int id, ProductDTO product);
        Task<int> DeleteProduct(int id);
    }
}