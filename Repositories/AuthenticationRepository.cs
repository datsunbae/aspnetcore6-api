using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;

namespace api_aspnetcore6.Repositories
{
    public class AuthenticationRepository : GenericRepository<ApplicationUser>, IAuthenticationRepository
    {
        public AuthenticationRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}