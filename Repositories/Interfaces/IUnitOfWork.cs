namespace api_aspnetcore6.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IAuthenticationRepository Users { get; }
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        void SaveChanges();
        void Rollback();
        Task SaveChangesAsync();
        Task RollbackAsync();
    }
}