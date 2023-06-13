using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace api_aspnetcore6.Dtos.Product
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int IdCategory { get; set; }
    }
}