using System.Reflection;
using api_aspnetcore6.Caching.Interfaces;
using api_aspnetcore6.Dtos;
using api_aspnetcore6.Dtos.Category;
using api_aspnetcore6.Helper;
using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;
using api_aspnetcore6.Services.Interfaces;
using AutoMapper;

namespace api_aspnetcore6.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IConfiguration configuration, ICacheService cacheService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _cacheService = cacheService;
            _mapper = mapper;
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

            _cacheService.RemoveData("categories");

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

            _cacheService.RemoveData("categories");
            return id;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategories(PagingParameters pagingParameters)
        {
            var cacheData = _cacheService.GetData<IEnumerable<CategoryResponse>>("categories");
            if (cacheData != null)
            {
                cacheData = PagedList<CategoryResponse>.ToPagedList(cacheData, pagingParameters.PageNumber, pagingParameters.PageSize);
                return cacheData;
            }

            _ = int.TryParse(_configuration["Caching:ExpirationTime"], out int time);
            var expirationTime = DateTimeOffset.Now.AddMinutes(time);

            var categories = await _unitOfWork.Categories!.GetAllAsync();

            var categoriesMapper = _mapper.Map<IEnumerable<CategoryResponse>>(categories);

            cacheData = PagedList<CategoryResponse>.ToPagedList(categoriesMapper, pagingParameters.PageNumber, pagingParameters.PageSize);

            _cacheService.SetData<IEnumerable<CategoryResponse>>("categories", cacheData, expirationTime);

            return cacheData;
        }

        public async Task<CategoryResponse> GetCategoryById(int id)
        {
            if (id == null)
            {
                throw new Exception("Id is required");
            }

            CategoryResponse categoryFilter = null;

            //Get category cache data
            var cacheData = _cacheService.GetData<IEnumerable<CategoryResponse>>("categories");
            if (cacheData != null)
            {
                categoryFilter = cacheData.Where(c => c.Id == id).FirstOrDefault();
            }

            if (categoryFilter != null)
            {
                return categoryFilter;
            }

            var getCategoryAsync = await _unitOfWork.Categories.GetAsync(c => c.Id == id);

            if (getCategoryAsync == null)
            {
                throw new Exception("Could not find categor");
            }

            categoryFilter = _mapper.Map<CategoryResponse>(getCategoryAsync);

            return categoryFilter;
        }

        public async Task<IEnumerable<CategoryResponse>> SearchCategories(string name, PagingParameters pagingParameters)
        {
            var categories = await GetAllCategories(pagingParameters);

            var categoriesMapper = _mapper.Map<IEnumerable<CategoryResponse>>(categories);

            IQueryable<CategoryResponse> categoriesQuery = categoriesMapper.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                categoriesQuery.Where(c => c.Name == name);
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

            _cacheService.RemoveData("categories");

            var response = _mapper.Map<CategoryResponse>(category);

            return response;
        }
    }
}