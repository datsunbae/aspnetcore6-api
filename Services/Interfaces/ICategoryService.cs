using api_aspnetcore6.Dtos;
using api_aspnetcore6.Dtos.Category;

namespace api_aspnetcore6.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategories(PagingParameters pagingParameters);
        Task<IEnumerable<CategoryResponse>> SearchCategories(string name, PagingParameters pagingParameters);
        Task<CategoryResponse> GetCategoryById(int id);
        Task<CategoryResponse> AddCategory(CategoryDTO categoryDTO);
        Task<CategoryResponse> UpdateCategory(int id, CategoryRequest categoryRequest);
        Task<int> DeleteCategory(int id);
    }
}