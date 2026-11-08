using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel;

namespace High_Traffic_E_Commerce_Inventory_Order_System.interfaces
{
    public interface IProductServices
    {
        
        public Task<List<GetProducts>> GetAllProducts(CancellationToken cancellationToken);
        public Task<GetProducts> SearchProduct(int productId,CancellationToken cancellationToken);
        public Task CreateProduct(CreateProduct product, CancellationToken cancellationToken);
        public Task DeleteProduct(int productId, CancellationToken cancellationToken);
        public Task UpdateProduct(int productId,UpdateProduct product, CancellationToken cancellationToken);  

    }
}
