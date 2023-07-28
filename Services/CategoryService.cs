using System.Reflection;
using api_aspnetcore6.Dtos;
using api_aspnetcore6.Dtos.Category;
using api_aspnetcore6.Helper;
using api_aspnetcore6.Helpers;
using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;
using api_aspnetcore6.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;

namespace api_aspnetcore6.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private const string categoryCacheKey = "categories";
        private readonly IDistributedCache _cache;
        private static readonly SemaphoreSlim semaphore = new(1, 1);
        public CategoryService(IUnitOfWork unitOfWork, IConfiguration configuration, IDistributedCache cache, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        public async Task<CategoryResponse> AddCategory(CategoryDTO request)
        {
            if (request == null)
            {
                throw new Exception("Invalid request");
            }

            var category = _mapper.Map<Category>(request);

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove(categoryCacheKey);

            var response = await GetCategoryById(category.Id);

            return response;
        }

        public async Task<int> DeleteCategory(int id)
        {
            if (id == null)
            {
                throw new Exception("Id is required");
            }

            var category = await _unitOfWork.Categories.GetAsync(c => c.Id == id);

            if (category == null)
            {
                throw new Exception("Category not found");
            }

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove(categoryCacheKey);
            return id;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategories(PagingParameters pagingParameters)
        {

            var cacheData = _cache.TryGetValue(categoryCacheKey, out IEnumerable<CategoryResponse>? categories);

            if (!cacheData)
            {
                try
                {
                    await semaphore.WaitAsync();
                    var categoryList = await _unitOfWork.Categories!.GetAllAsync();
                    categories = _mapper.Map<IEnumerable<CategoryResponse>>(categoryList);

                    var cacheEntryOptions = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                            .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600));
                    await _cache.SetAsync(categoryCacheKey, categories, cacheEntryOptions);
                }
                finally
                {
                    semaphore.Release();
                }
            }

            categories = PagedList<CategoryResponse>.ToPagedList(categories, pagingParameters.PageNumber, pagingParameters.PageSize);

            return categories;
        }

        public async Task<CategoryResponse> GetCategoryById(int id)
        {
            if (id == null)
            {
                throw new Exception("Id is required");
            }

            CategoryResponse categoryFilter = null;

            //Get category cache data
            var cacheData = _cache.TryGetValue(categoryCacheKey, out IEnumerable<CategoryResponse>? categories);
            if (cacheData)
            {
                categoryFilter = categories.Where(c => c.Id == id).FirstOrDefault();

                if (categoryFilter != null)
                {
                    return categoryFilter;
                }
            }

            var getCategoryAsync = await _unitOfWork.Categories.GetAsync(c => c.Id == id);

            if (getCategoryAsync == null)
            {
                throw new Exception("Could not find category");
            }

            categoryFilter = _mapper.Map<CategoryResponse>(getCategoryAsync);

            return categoryFilter;
        }

        public async Task<IEnumerable<CategoryResponse>> SearchCategories(string name, PagingParameters pagingParameters)
        {
            var categories = await GetAllCategories(pagingParameters);

            IQueryable<CategoryResponse> categoriesQuery = categories.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                categoriesQuery.Where(c => c.Name.ToLower().Contains(name.ToLower()));
            }

            return PagedList<CategoryResponse>.ToPagedList(categoriesQuery.ToList(), pagingParameters.PageNumber, pagingParameters.PageSize);
        }

        public async Task<CategoryResponse> UpdateCategory(int id, CategoryRequest categoryRequest)
        {
            if (id == null || categoryRequest == null)
            {
                throw new Exception("Invalid request");
            }

            var category = await _unitOfWork.Categories.GetAsync(c => c.Id == id);

            if (category == null)
            {
                throw new Exception("Not found Category");
            }

            foreach (PropertyInfo prop in categoryRequest.GetType().GetProperties())
            {
                if (prop.Name == "Id")
                {
                    continue;
                }

                if (prop.GetValue(categoryRequest) != null)
                {
                    category.GetType().GetProperty(prop.Name)?.SetValue(category, prop.GetValue(categoryRequest));
                }
            }

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove(categoryCacheKey);

            var response = _mapper.Map<CategoryResponse>(category);

            return response;
        }
    }
}