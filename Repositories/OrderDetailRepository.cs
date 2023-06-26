using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;

namespace api_aspnetcore6.Repositories
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

    }
}