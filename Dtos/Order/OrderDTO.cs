using System.ComponentModel.DataAnnotations;
using api_aspnetcore6.Dtos.OrderDetail;
using api_aspnetcore6.Models;

namespace api_aspnetcore6.Dtos.Order
{
    public class OrderDTO
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a user id")]
        public string UserId { get; set; }
        public bool IsPayment { get; set; } = false;
        [Required(ErrorMessage = "Please enter order details")]
        public virtual List<OrderDetailDTO> OrderDetails { get; set; }
    }
}