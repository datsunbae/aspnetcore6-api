using api_aspnetcore6.Dtos;
using api_aspnetcore6.Models;

namespace api_aspnetcore6.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategories(PagingParameters pagingParameters);
        Task<List<Category>> SearchCategories(string name, PagingParameters pagingParameters);
        Task<Category> GetCategoryById(int id);
        Task<Category> AddCategory(Category category);
        Task<Category> UpdateCategory(int id, Category category);
        Task<int> DeleteCategory(int id);
    }
}