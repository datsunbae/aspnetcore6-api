using api_aspnetcore6.Dtos;
using api_aspnetcore6.Dtos.Order;
using api_aspnetcore6.Models;
using api_aspnetcore6.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_aspnetcore6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] PagingParameters pagingParameters)
        {
            try
            {
                var response = await _orderService.GetAllOrders(pagingParameters);
                _logger.LogInformation($"SeriLog - Order (GetAllOrders): Get all orders successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Order (GetAllOrders): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdAsync(int id)
        {
            try
            {
                var response = await _orderService.GetOrderById(id);

                if (response == null)
                {
                    _logger.LogWarning($"SeriLog - Order (GetOrderByIdAsync): Order not found");
                    return NotFound("Order not found");
                }

                _logger.LogInformation($"SeriLog - Order (GetOrderByIdAsync): Order found (IdOrder: {response.Id})");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Order (GetOrderByIdAsync): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [HttpGet("GetOrdersByUserId")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
        public async Task<IActionResult> GetOrdersByUserId([FromQuery] PagingParameters pagingParameters)
        {
            try
            {
                var idUser = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                var response = await _orderService.GetOrderByUserId(idUser, pagingParameters);

                _logger.LogInformation($"SeriLog - Order (GetOrdersByUserId): Get all orders by user id successfully");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Order (GetOrdersByUserId): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchProduct(double? fromTotal, double? toTotal, [FromQuery] PagingParameters pagingParameters)
        {
            try
            {
                var idUser = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                var role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                var response = await _orderService.SearchOrder(idUser, role, fromTotal, toTotal, pagingParameters);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Order (SearchProduct): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> AddOrder(OrderDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"SeriLog - Order (AddOrder): Bad request");
                    return BadRequest();
                }

                var response = await _orderService.AddOrder(request);
                _logger.LogInformation($"SeriLog - Order (AddOrder): Add order successfully (Id: {response.Id})");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Order (AddOrder): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = $"{UserRoles.Admin}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid || id == null)
                {
                    _logger.LogWarning($"SeriLog - Order (UpdateOrder): Order not found");
                    return BadRequest();
                }

                var reponse = await _orderService.UpdateOrder(id, request);

                _logger.LogInformation($"SeriLog - Order (UpdateOrder): Update order (Id: {id}) successfully");
                return Ok(reponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Order (UpdateOrder): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

    }
}