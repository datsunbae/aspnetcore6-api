using api_aspnetcore6.Helper;
using api_aspnetcore6.Caching.Interfaces;
using api_aspnetcore6.Dtos;
using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api_aspnetcore6.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _configuration;
        public ProductRepository(DatabaseContext dbContext, ICacheService cacheService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
            _configuration = configuration;
        }

        public async Task<List<Product>> GetAllProducts(PagingParameters pagingParameters)
        {
            var cacheData = _cacheService.GetData<List<Product>>("products");
            if (cacheData != null)
            {
                return cacheData;
            }

            _ = int.TryParse(_configuration["Caching:ExpirationTime"], out int time);
            var expirationTime = DateTimeOffset.Now.AddMinutes(time);
            var products = await _dbContext.Products!.ToListAsync();
            cacheData = PagedList<Product>.ToPagedList(products, pagingParameters.PageNumber, pagingParameters.PageSize);

            _cacheService.SetData<List<Product>>("products", cacheData, expirationTime);

            return cacheData;
        }

        public async Task<List<Product>> SearchProduct(string? name, double? priceFrom, double? priceTo, int? idCategory, PagingParameters pagingParameters){
            
            var products = await GetAllProducts(pagingParameters);

            if(products == null){
                return null;
            }

            IQueryable<Product> productsQuery = products.AsQueryable();

            if(!string.IsNullOrEmpty(name)){
                productsQuery = productsQuery.Where(p => p.Name == name);
            }
 
            if(priceFrom.HasValue && priceTo > 0){
                productsQuery = productsQuery.Where(p => p.Price == priceFrom);
            }

            if(priceTo.HasValue && priceFrom > 0){
                productsQuery = productsQuery.Where(p => p.Price == priceTo);
            }

            if(priceTo.HasValue){
                productsQuery = productsQuery.Where(p => p.IdCategory == idCategory);
            }

            return await productsQuery.ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            Product productFilter = null;

            var cacheData = _cacheService.GetData<List<Product>>("products");
            if (cacheData != null)
            {
                productFilter = cacheData.Where(c => c.Id == id).FirstOrDefault();
            }

            if (productFilter != null)
            {
                return productFilter;
            }

            productFilter = await _dbContext.Products.FirstOrDefaultAsync(c => c.Id == id);

            if (productFilter == null)
            {
                throw new Exception("Could not find product");
            }

            return productFilter;
        }

        public async Task<Product> AddProduct(Product product)
        {

            var checkCategory = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == product.IdCategory);

            if (checkCategory == null)
            {
                throw new Exception("Category not found");
            }

            await _dbContext.Products.AddAsync(product);
            _cacheService.RemoveData("products");
            await _dbContext.SaveChangesAsync();
            var response = await GetProductById(product.Id);
            return response;
        }

        public async Task<Product> UpdateProduct(int id, Product product)
        {
            var data = await GetProductById(id);

            if (data == null)
            {
                throw new Exception("Product not found");
            }

            var checkCategory = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == product.IdCategory);

            if (checkCategory == null)
            {
                throw new Exception("Category not found");
            }

            data.Name = product.Name;
            data.Quantity = product.Quantity;
            data.Price = product.Price;
            data.IdCategory = product.IdCategory;

            _cacheService.RemoveData("products");
            await _dbContext.SaveChangesAsync();

            return data;
        }
        public async Task<int> DeleteProduct(int id)
        {
            var data = await _dbContext.Products.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
            {
                throw new Exception("Product not found");
            }

            _dbContext.Products.Remove(data);
            _cacheService.RemoveData("products");
            await _dbContext.SaveChangesAsync();

            return data.Id;
        }
    }
}