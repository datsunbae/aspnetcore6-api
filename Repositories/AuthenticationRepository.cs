using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api_aspnetcore6.Repositories
{
    public class AuthenticationRepository : GenericRepository<ApplicationUser>, IAuthenticationRepository
    {
        public AuthenticationRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<ApplicationUser> GetUser(string id)
        {
            var data = await _dbContext.ApplicationUsers.Where(c => c.Id == id).FirstOrDefaultAsync();
            return data;
        }
    }
}