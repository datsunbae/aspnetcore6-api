using System.Linq.Expressions;
using api_aspnetcore6.Dtos.Order;
using api_aspnetcore6.Models;

namespace api_aspnetcore6.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersIncludeOrderDetails();
        Task<IEnumerable<Order>> GetOrdersIncludeOrderDetailsOfUser(string userId);
        Task<Order> GetOrderByIdAsync(int id);
    }
}