using System.Linq.Expressions;
using api_aspnetcore6.Dtos.Order;
using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api_aspnetcore6.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersIncludeOrderDetails()
        {
            var data = await _dbContext.Orders.Include(o => o.OrderDetails).ToListAsync();
            return data;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {

            var data = await _dbContext.Orders.Where(o => o.Id == id).Include(o => o.OrderDetails).FirstOrDefaultAsync();
            return data;
        }

        public async Task<IEnumerable<Order>> GetOrdersIncludeOrderDetailsOfUser(string userId)
        {
            var data = await _dbContext.Orders.Where(o => o.UserId == userId).Include(o => o.OrderDetails).ToListAsync();
            return data;
        }
    }
}