using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_aspnetcore6.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(255)]
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Provided phone number not valid")]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Order date is required")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public double TotalAmount { get; set; }
        [Required(ErrorMessage = "Status order is required")]
        [EnumDataType(typeof(StatusOrder), ErrorMessage = "Invalid status order value")]
        public string Status { get; set; } = StatusOrder.Open;
        public bool IsPayment { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
    }
}