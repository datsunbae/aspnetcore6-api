using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace api_aspnetcore6.Dtos
{
    public class ProductDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [Range(0, Double.MaxValue)]
        public double Price { get; set; }
        [DefaultValue(0)]
        public int Quantity { get; set; }
        [Required]
        public int IdCategory { get; set; }
    }
}