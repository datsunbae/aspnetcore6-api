
using System.Text.Json.Serialization;
using api_aspnetcore6.Dtos.OrderDetail;

namespace api_aspnetcore6.Dtos.Order
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public bool IsPayment { get; set; }
        public double TotalAmount { get; set; }
        public string UserId { get; set; }
        public List<OrderDetailReponse> OrderDetails { get; set; }
    }
}