using api_aspnetcore6.Models;

namespace api_aspnetcore6.Repositories.Interfaces
{
    public interface IAuthenticationRepository : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetUser(string userId);
    }
}