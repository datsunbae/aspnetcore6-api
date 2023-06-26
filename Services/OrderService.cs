using System.Reflection;
using api_aspnetcore6.Caching.Interfaces;
using api_aspnetcore6.Dtos;
using api_aspnetcore6.Dtos.Order;
using api_aspnetcore6.Dtos.OrderDetail;
using api_aspnetcore6.Helper;
using api_aspnetcore6.Models;
using api_aspnetcore6.Repositories.Interfaces;
using api_aspnetcore6.Services.Interfaces;
using AutoMapper;

namespace api_aspnetcore6.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ISendMailService _sendMailJobService;

        public OrderService(IUnitOfWork unitOfWork, IConfiguration configuration, ICacheService cacheService, IMapper mapper, ISendMailService sendMailJobService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _cacheService = cacheService;
            _sendMailJobService = sendMailJobService;
        }
        public async Task<OrderResponse> AddOrder(OrderDTO request)
        {
            if (request == null)
            {
                throw new Exception("Invalid request");
            }

            double total = 0;
            List<OrderDetailDTO> listOrderDetails = new List<OrderDetailDTO>();

            foreach (var item in request.OrderDetails)
            {
                var product = await _unitOfWork.Products.GetAsync(p => p.Id == item.IdProduct);
                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                if (item.UnitPrice == null)
                {
                    item.UnitPrice = product.Price;
                }

                listOrderDetails.Add(item);

                total += (double)item.UnitPrice * item.Quantity;
            }

            var orderMapper = _mapper.Map<Order>(request);

            orderMapper.TotalAmount = total;
            orderMapper.Status = StatusOrder.Open;
            orderMapper.OrderDate = DateTime.UtcNow;

            await _unitOfWork.Orders.AddAsync(orderMapper);
            await _unitOfWork.SaveChangesAsync();

            var user = await _unitOfWork.Users.GetUser(request.UserId);

            //Send mail notification
            var toEmail = new string[] { user.Email };
            var subject = "Thank you for your order";
            var body = $"<h1>Hi, {user.UserName}</h1><br><p>Thank you, we will send your order as soon as possible!</p>";
            var message = new MailMessage(toEmail, subject, body);
            await _sendMailJobService.SendEmailAsync(message);

            _cacheService.RemoveData("orders");

            var response = await GetOrderById(orderMapper.Id);

            return response;
        }

        public async Task<IEnumerable<OrderResponse>> GetAllOrders(PagingParameters pagingParameters)
        {
            var cacheData = _cacheService.GetData<IEnumerable<OrderResponse>>("orders");
            if (cacheData != null)
            {
                cacheData = PagedList<OrderResponse>.ToPagedList(cacheData, pagingParameters.PageNumber, pagingParameters.PageSize);
                return cacheData;
            }

            _ = int.TryParse(_configuration["Caching:ExpirationTime"], out int time);
            var expirationTime = DateTimeOffset.Now.AddMinutes(time);

            var orders = await _unitOfWork.Orders!.GetOrdersIncludeOrderDetails();

            var orderResponse = MapperListOrderResponse(orders.ToList());

            cacheData = PagedList<OrderResponse>.ToPagedList(orderResponse, pagingParameters.PageNumber, pagingParameters.PageSize);

            _cacheService.SetData<IEnumerable<OrderResponse>>("orders", cacheData, expirationTime);

            return cacheData;
        }

        public async Task<OrderResponse> GetOrderById(int id)
        {
            if (id == null)
            {
                throw new Exception("Id is required");
            }

            OrderResponse orderFilter = null;

            var cacheData = _cacheService.GetData<IEnumerable<OrderResponse>>("orders");

            if (cacheData != null)
            {
                orderFilter = cacheData.Where(o => o.Id == id).FirstOrDefault();
            }

            if (orderFilter != null)
            {
                return orderFilter;
            }

            var order = await _unitOfWork.Orders.GetOrderByIdAsync(id);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            var response = MapperOrderResponse(order);

            return response;
        }

        public async Task<IEnumerable<OrderResponse>> GetOrderByUserId(string userId, PagingParameters pagingParameters)
        {
            var cacheData = _cacheService.GetData<IEnumerable<OrderResponse>>("orders");
            if (cacheData != null)
            {
                cacheData = cacheData.Where(o => o.UserId == userId).ToList();
                cacheData = PagedList<OrderResponse>.ToPagedList(cacheData, pagingParameters.PageNumber, pagingParameters.PageSize);
                return cacheData;
            }

            var orders = await _unitOfWork.Orders.GetOrdersIncludeOrderDetailsOfUser(userId);

            var orderResponse = MapperListOrderResponse(orders.ToList());

            return PagedList<OrderResponse>.ToPagedList(orderResponse, pagingParameters.PageNumber, pagingParameters.PageSize); ;
        }

        public async Task<IEnumerable<OrderResponse>> SearchOrder(string userId, string role, double? fromTotal, double? toTotal, PagingParameters pagingParameters)
        {
            var orders = await GetAllOrders(pagingParameters);

            var orderMapper = _mapper.Map<IEnumerable<Order>>(orders);

            IQueryable<Order> ordersQuery = orderMapper.AsQueryable();

            if (role.Equals(UserRoles.User))
            {
                ordersQuery = ordersQuery.Where(x => x.UserId == userId);
            }

            if (fromTotal.HasValue && fromTotal > 0)
            {
                ordersQuery = ordersQuery.Where(p => p.TotalAmount >= fromTotal);
            }

            if (toTotal.HasValue && toTotal > 0)
            {
                ordersQuery = ordersQuery.Where(p => p.TotalAmount <= toTotal);
            }

            var orderResponse = MapperListOrderResponse(ordersQuery.ToList());

            return PagedList<OrderResponse>.ToPagedList(orderResponse, pagingParameters.PageNumber, pagingParameters.PageSize);
        }

        public async Task<OrderResponse> UpdateOrder(int id, OrderRequest request)
        {
            if (id == null || request == null)
            {
                throw new Exception("Invalid request");
            }

            var order = await _unitOfWork.Orders.GetOrderByIdAsync(id);

            if (order == null)
            {
                throw new Exception("Order not found");
            }

            foreach (PropertyInfo prop in request.GetType().GetProperties())
            {
                if (prop.Name == "Id")
                {
                    continue;
                }

                if (prop.GetValue(request) != null)
                {
                    order.GetType().GetProperty(prop.Name)?.SetValue(order, prop.GetValue(request));
                }
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            _cacheService.RemoveData("orders");

            var response = MapperOrderResponse(order);

            return response;
        }

        private OrderResponse MapperOrderResponse(Order order)
        {
            var response = new OrderResponse
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                IsPayment = order.IsPayment,
                TotalAmount = order.TotalAmount,
                UserId = order.UserId,
                OrderDetails = order.OrderDetails.Select(detail => new OrderDetailReponse
                {
                    Id = detail.Id,
                    Quantity = (int)detail.Quantity,
                    UnitPrice = (int)detail.UnitPrice,
                    IdOrder = (int)detail.IdOrder,
                    IdProduct = (int)detail.IdProduct
                }).ToList()
            };

            return response;
        }

        private List<OrderResponse> MapperListOrderResponse(List<Order> orders)
        {
            var reponse = orders.Select(x => new OrderResponse
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                Address = x.Address,
                PhoneNumber = x.PhoneNumber,
                OrderDate = x.OrderDate,
                Status = x.Status,
                IsPayment = x.IsPayment,
                TotalAmount = x.TotalAmount,
                UserId = x.UserId,
                OrderDetails = x.OrderDetails.Select(detail => new OrderDetailReponse
                {
                    Id = detail.Id,
                    Quantity = (int)detail.Quantity,
                    UnitPrice = (int)detail.UnitPrice,
                    IdOrder = (int)detail.IdOrder,
                    IdProduct = (int)detail.IdProduct
                }).ToList()
            }).ToList();

            return reponse;
        }
    }
}