using api_aspnetcore6.Dtos;
using api_aspnetcore6.Dtos.Order;

namespace api_aspnetcore6.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponse>> GetAllOrders(PagingParameters pagingParameters);
        Task<IEnumerable<OrderResponse>> SearchOrder(string userId, string role, double? fromTotal, double? toTotal, PagingParameters pagingParameters);
        Task<OrderResponse> GetOrderById(int id);
        Task<IEnumerable<OrderResponse>> GetOrderByUserId(string id, PagingParameters pagingParameters);
        Task<OrderResponse> AddOrder(OrderDTO request);
        Task<OrderResponse> UpdateOrder(int id, OrderRequest request);
    }
}