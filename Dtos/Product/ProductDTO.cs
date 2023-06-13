using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace api_aspnetcore6.Dtos.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a product name")]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a price product")]
        [Range(0, Double.MaxValue)]
        public double Price { get; set; }
        [DefaultValue(0)]
        public int Quantity { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a category id")]
        public int IdCategory { get; set; }
    }
}