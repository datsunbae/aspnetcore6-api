namespace api_aspnetcore6.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoryRepository Categories { get; }
        IAuthenticationRepository Users { get; }
        void SaveChanges();
        void Rollback();
        Task SaveChangesAsync();
        Task RollbackAsync();
    }
}