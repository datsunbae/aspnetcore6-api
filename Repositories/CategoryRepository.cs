using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;

namespace api_aspnetcore6.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}