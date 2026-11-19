namespace High_Traffic_E_Commerce_Inventory_Order_System.Exceptions
{
    public class OrderStateConflictException: Exception
    {
        public OrderStateConflictException(string message) : base(message) 
        { 
        }
    }
}
