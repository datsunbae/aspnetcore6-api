using System.ComponentModel.DataAnnotations;

namespace api_aspnetcore6.Dtos.OrderDetail
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a quantity")]
        public int Quantity { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a unit price")]
        public double UnitPrice { get; set; }
        public int? IdOrder { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a product id")]
        public int IdProduct { get; set; }
    }
}