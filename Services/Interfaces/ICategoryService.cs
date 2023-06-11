using api_aspnetcore6.Dtos;

namespace api_aspnetcore6.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetALlCategories(PagingParameters pagingParameters);
        Task<List<CategoryDTO>> SearchCategories(string name, PagingParameters pagingParameters);
        Task<CategoryDTO> GetCategoryById(int id);
        Task<CategoryDTO> AddCategory(CategoryDTO category);
        Task<CategoryDTO> UpdateCategory(int id, CategoryDTO category);
        Task<int> DeleteCategory(int id);
    }
}