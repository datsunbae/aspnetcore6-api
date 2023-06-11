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
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
    }
}