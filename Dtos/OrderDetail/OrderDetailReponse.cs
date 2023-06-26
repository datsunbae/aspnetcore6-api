using api_aspnetcore6.Dtos.Order;
using api_aspnetcore6.Dtos.Product;


namespace api_aspnetcore6.Dtos.OrderDetail
{
    public class OrderDetailReponse
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public int IdOrder { get; set; }
        public int IdProduct { get; set; }
        public virtual OrderResponse? Order { get; set; }
        public virtual ProductResponse? Product { get; set; }
    }
}