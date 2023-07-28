using System.Reflection;
using api_aspnetcore6.Dtos;
using api_aspnetcore6.Dtos.Product;
using api_aspnetcore6.Helper;
using api_aspnetcore6.Helpers;
using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;
using api_aspnetcore6.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;

namespace api_aspnetcore6.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private const string productCacheKey = "products";
        private readonly IDistributedCache _cache;
        private static readonly SemaphoreSlim semaphore = new(1, 1);
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IConfiguration configuration, IDistributedCache cache, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<ProductResponse> AddProduct(ProductDTO request)
        {
            if (request == null)
            {
                throw new Exception("Invalid request");
            }

            var product = _mapper.Map<Product>(request);

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove(productCacheKey);

            var response = await GetProductById(product.Id);

            return response;
        }

        public async Task<int> DeleteProduct(int id)
        {
            if (id == null)
            {
                throw new Exception("Id is required");
            }

            var product = await _unitOfWork.Products.GetAsync(c => c.Id == id);

            if (product == null)
            {
                throw new Exception("Product not found");
            }

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove(productCacheKey);
            return id;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProducts(PagingParameters pagingParameters)
        {
            var cacheData = _cache.TryGetValue(productCacheKey, out IEnumerable<ProductResponse>? products);

            if (!cacheData)
            {
                try
                {
                    await semaphore.WaitAsync();
                    var productList = await _unitOfWork.Categories!.GetAllAsync();
                    products = _mapper.Map<IEnumerable<ProductResponse>>(productList);

                    var cacheEntryOptions = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                            .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600));
                    await _cache.SetAsync(productCacheKey, products, cacheEntryOptions);
                }
                finally
                {
                    semaphore.Release();
                }
            }

            products = PagedList<ProductResponse>.ToPagedList(products, pagingParameters.PageNumber, pagingParameters.PageSize);

            return products;
        }

        public async Task<ProductResponse> GetProductById(int id)
        {
            if (id == null)
            {
                throw new Exception("Id is required");
            }

            ProductResponse productFilter = null;

            //Get product cache data
            var cacheData = _cache.TryGetValue(productCacheKey, out IEnumerable<ProductResponse>? products);
            if (cacheData)
            {
                productFilter = products.Where(c => c.Id == id).FirstOrDefault();

                if (productFilter != null)
                {
                    return productFilter;
                }
            }

            var getProductAsync = await _unitOfWork.Categories.GetAsync(c => c.Id == id);

            if (getProductAsync == null)
            {
                throw new Exception("Could not find product");
            }

            productFilter = _mapper.Map<ProductResponse>(getProductAsync);

            return productFilter;
        }

        public async Task<IEnumerable<ProductResponse>> SearchProduct(string? name, double? priceFrom, double? priceTo, int? idCategory, PagingParameters pagingParameters)
        {
            var products = await GetAllProducts(pagingParameters);

            IQueryable<ProductResponse> productsQuery = products.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }

            if (priceFrom.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= priceFrom);
            }

            if (priceTo.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= priceTo);
            }

            if (idCategory.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.IdCategory == idCategory);
            }

            return PagedList<ProductResponse>.ToPagedList(productsQuery.ToList(), pagingParameters.PageNumber, pagingParameters.PageSize);
        }

        public async Task<ProductResponse> UpdateProduct(int id, ProductRequest productRequest)
        {
            if (id == null || productRequest == null)
            {
                throw new Exception("Invalid request");
            }

            var product = await _unitOfWork.Products.GetAsync(c => c.Id == id);

            if (product == null)
            {
                throw new Exception("Product not found");
            }

            var checkCategory = await _unitOfWork.Categories.GetAsync(c => c.Id == productRequest.IdCategory);

            if (checkCategory == null)
            {
                throw new Exception("Category not found");
            }

            foreach (PropertyInfo prop in productRequest.GetType().GetProperties())
            {
                if (prop.Name == "Id")
                {
                    continue;
                }

                if (prop.GetValue(productRequest) != null)
                {
                    product.GetType().GetProperty(prop.Name)?.SetValue(product, prop.GetValue(productRequest));
                }
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove(productCacheKey);

            var response = _mapper.Map<ProductResponse>(product);

            return response;
        }
    }
}