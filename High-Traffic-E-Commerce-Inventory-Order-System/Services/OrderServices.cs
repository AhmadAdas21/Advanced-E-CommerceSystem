using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Services
{
    public class OrderServices:IOrderServices
    {
        private readonly DataBaseContext _context;

        public OrderServices( DataBaseContext context)
        {
            context = _context;
        }

    }
}
