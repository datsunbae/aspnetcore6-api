using api_aspnetcore6.Dtos;
using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;
using api_aspnetcore6.Services.Interfaces;
using AutoMapper;

namespace api_aspnetcore6.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>> GetAllProducts(PagingParameters pagingParameters)
        {
            var data = await _productRepository.GetAllProducts(pagingParameters);
            var response = _mapper.Map<List<ProductDTO>>(data);
            return response;
        }

        public async Task<List<ProductDTO>> SearchProduct(string? name, double? priceFrom, double? priceTo, int? idCategory, PagingParameters pagingParameters)
        {
            var data = await _productRepository.SearchProduct(name, priceFrom, priceTo, idCategory, pagingParameters);
            var response = _mapper.Map<List<ProductDTO>>(data);
            return response;
        }

        public async Task<ProductDTO> GetProductById(int id)
        {
            if (id == null)
            {
                throw new Exception("Id is required");
            }

            var data = await _productRepository.GetProductById(id);
            var response = _mapper.Map<ProductDTO>(data);
            return response;
        }

        public async Task<ProductDTO> AddProduct(ProductDTO product)
        {
            var productMapper = _mapper.Map<Product>(product);

            var data = await _productRepository.AddProduct(productMapper);

            var response = _mapper.Map<ProductDTO>(data);

            return response;
        }

        public async Task<ProductDTO> UpdateProduct(int id, ProductDTO product)
        {
            var productMapper = _mapper.Map<Product>(product);

            var data = await _productRepository.UpdateProduct(id, productMapper);

            var response = _mapper.Map<ProductDTO>(data);

            return response;
        }

        public async Task<int> DeleteProduct(int id)
        {
            if (id == null)
            {
                throw new Exception("Id is required");
            }

            return await _productRepository.DeleteProduct(id);
        }

    }
}