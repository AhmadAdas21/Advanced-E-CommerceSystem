using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Services
{
    public class ProductServices:IProductServices
    {
        private readonly DataBaseContext _dataBaseContext;
        public ProductServices(DataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }
        public async Task<List<GetProducts>> GetAllProducts(CancellationToken cancellationToken)
        {
            var products = _dataBaseContext.Products.FindAsync(cancellationToken);
            if(products==null)
            {
               
            }
        }

    }
}
