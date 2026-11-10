using High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel;

namespace High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public int OrderQuantity{get;set;}
        public decimal TotalPrice { get; set; }



        
    }
}
