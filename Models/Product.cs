using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_aspnetcore6.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [Range(0, Double.MaxValue)]
        public double Price { get; set; }
        [DefaultValue(0)]
        public int Quantity { get; set; }
        public int? IdCategory { get; set; }

        [ForeignKey("IdCategory")]
        public virtual Category? category { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
    }
}