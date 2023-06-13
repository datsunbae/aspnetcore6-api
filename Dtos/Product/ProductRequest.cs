using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace api_aspnetcore6.Dtos.Product
{
    public class ProductRequest
    {
        [MaxLength(255)]
        public string Name { get; set; }
        [Range(0, Double.MaxValue)]
        public double? Price { get; set; }
        [DefaultValue(0)]
        public int? Quantity { get; set; }
        public int? IdCategory { get; set; }
    }
}