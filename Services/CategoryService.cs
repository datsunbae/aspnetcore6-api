using api_aspnetcore6.Models;
using AutoMapper;
using api_aspnetcore6.Services.Interfaces;
using api_aspnetcore6.Dtos;
using api_aspnetcore6.Repositories.Interfaces;

namespace api_aspnetcore6.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryDTO>> GetALlCategories(PagingParameters pagingParameters)
        {
            var data = await _categoryRepository.GetAllCategories(pagingParameters);
            var response = _mapper.Map<List<CategoryDTO>>(data);
            return response;
        }

        public async Task<CategoryDTO> GetCategoryById(int id)
        {
            if (id == null)
            {
                throw new Exception("Id is required");
            }

            var data = await _categoryRepository.GetCategoryById(id);
            var response = _mapper.Map<CategoryDTO>(data);
            return response;
        }

        public async Task<CategoryDTO> AddCategory(CategoryDTO category)
        {
            var categoryMapper = _mapper.Map<Category>(category);

            var data = await _categoryRepository.AddCategory(categoryMapper);

            var response = _mapper.Map<CategoryDTO>(data);

            return response;
        }

        public async Task<CategoryDTO> UpdateCategory(int id, CategoryDTO category)
        {
            var categoryMapper = _mapper.Map<Category>(category);

            var data = await _categoryRepository.UpdateCategory(id, categoryMapper);

            var response = _mapper.Map<CategoryDTO>(data);

            return response;
        }

        public async Task<int> DeleteCategory(int id)
        {
            if (id == null)
            {
                throw new Exception("Id is required");
            }

            return await _categoryRepository.DeleteCategory(id);
        }

        public async Task<List<CategoryDTO>> SearchCategories(string name, PagingParameters pagingParameters)
        {
            var response = await _categoryRepository.SearchCategories(name, pagingParameters);

            var responseMapper = _mapper.Map<List<CategoryDTO>>(response);

            return responseMapper;
        }
    }
}