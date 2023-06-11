using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_aspnetcore6.Models
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public double? UnitPrice { get; set; }
        public int? IdOrder { get; set; }
        public int? IdProduct { get; set; }

        [ForeignKey("IdOrder")]
        public virtual Order? Order { get; set; }
        [ForeignKey("IdProduct")]
        public virtual Product? Product { get; set; }
    }
}