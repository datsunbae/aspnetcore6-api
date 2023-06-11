using api_aspnetcore6.Helper;
using api_aspnetcore6.Caching.Interfaces;
using api_aspnetcore6.Dtos;
using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api_aspnetcore6.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DatabaseContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _configuration;
        public CategoryRepository(DatabaseContext dbContext, ICacheService cacheService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
            _configuration = configuration;
        }

        public async Task<List<Category>> GetAllCategories(PagingParameters pagingParameters)
        {
            var cacheData = _cacheService.GetData<List<Category>>("category");
            if (cacheData != null)
            {
                return cacheData;
            }

            _ = int.TryParse(_configuration["Caching:ExpirationTime"], out int time);
            var expirationTime = DateTimeOffset.Now.AddMinutes(time);
            var categories = await _dbContext.Categories!.ToListAsync();
            cacheData = PagedList<Category>.ToPagedList(categories, pagingParameters.PageNumber, pagingParameters.PageSize);

            _cacheService.SetData<List<Category>>("category", cacheData, expirationTime);

            return cacheData;
        }

        public async Task<Category> GetCategoryById(int id)
        {
            Category categoryFilter = null;

            var cacheData = _cacheService.GetData<List<Category>>("category");
            if (cacheData != null)
            {
                categoryFilter = cacheData.Where(c => c.Id == id).FirstOrDefault();
            }

            if (categoryFilter != null)
            {
                return categoryFilter;
            }

            categoryFilter = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (categoryFilter == null)
            {
                throw new Exception("Could not find category");
            }

            return categoryFilter;
        }
        public async Task<Category> AddCategory(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            _cacheService.RemoveData("category");
            await _dbContext.SaveChangesAsync();
            var response = await GetCategoryById(category.Id);
            return response;
        }

        public async Task<Category> UpdateCategory(int id, Category category)
        {
            var data = await GetCategoryById(id);

            if (data == null)
            {
                throw new Exception("Not found Category");
            }

            data.Name = category.Name;

            _cacheService.RemoveData("category");
            await _dbContext.SaveChangesAsync();

            return data;
        }

        public async Task<int> DeleteCategory(int id)
        {
            var data = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
            {
                throw new Exception("Category not found");
            }

            _dbContext.Categories.Remove(data);
            _cacheService.RemoveData("category");
            await _dbContext.SaveChangesAsync();

            return data.Id;
        }

        public async Task<List<Category>> SearchCategories(string name, PagingParameters pagingParameters)
        {
            var categories = await GetAllCategories(pagingParameters);

            IQueryable<Category> categoriesQuery = categories.AsQueryable();

            if(!string.IsNullOrEmpty(name)){
                categoriesQuery.Where(c => c.Name == name);
            }

            return PagedList<Category>.ToPagedList(categoriesQuery.ToList(), pagingParameters.PageNumber, pagingParameters.PageSize);
        }
    }
}