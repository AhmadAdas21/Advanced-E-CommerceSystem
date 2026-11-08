using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel;
using Microsoft.EntityFrameworkCore;

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
            var products =await  _dataBaseContext.Products.ToListAsync();
            if(products==null)
            {
                throw new Exception("there's no products");
            }
            return products.Select(product=> new GetProducts
            {
                ProductDescription = product.ProductDescription,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                SKU = product.SKU,
                AvailableStock = product.AvailableStock,
                IsActive = product.IsActive,

            }).ToList();
            
        }
        
        public async Task<GetProducts>SearchProduct(int productId, CancellationToken cancellationToken)
        {
            var product = await _dataBaseContext.Products.FirstOrDefaultAsync(product => product.ProductId == productId);
            if(product ==null )
            {
                throw new Exception("there's no product exist for this id");
               
            }
            return new GetProducts
            {
                ProductDescription = product.ProductDescription,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                SKU = product.SKU,
                AvailableStock = product.AvailableStock,
                IsActive = product.IsActive,

            };
        }
        public async Task CreateProduct(CreateProduct product, CancellationToken cancellationToken)
        {
            if (product == null)
            {
                throw new Exception("the create product dto is null ");
            }
            var newProduct = new Product
            {
                ProductDescription = product.ProductDescription,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                SKU = product.SKU,
                AvailableStock = (int)product.AvailableStock,
                IsActive = (bool)product.IsActive,
                

            };
            await _dataBaseContext.Products.AddAsync(newProduct,cancellationToken);
            await _dataBaseContext.SaveChangesAsync(cancellationToken);
            
        }
        public async Task DeleteProduct(int productId, CancellationToken cancellationToken)
        {
            var product = await _dataBaseContext.Products.FirstOrDefaultAsync(product => product.ProductId == productId);
            if (product == null)
            {
                throw new Exception("the product is not found");
            }
            _dataBaseContext.Products.Remove(product); 
            await _dataBaseContext.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdateProduct(int productId,UpdateProduct product,CancellationToken cancellationToken)
        {
            var existProduct=await _dataBaseContext.Products.FirstOrDefaultAsync(product=>product.ProductId == productId);
            if(existProduct==null)
            {
                throw new Exception("the product is not avaliable ");
            }
            if (product == null)
            {
                throw new Exception("the update product dto is null , enter valid values");
            }
            var newProduct = new Product
            {
                ProductName=product.ProductName,
                ProductPrice = product.ProductPrice,
                ProductDescription=product.ProductDescription,
                IsActive=(bool)product.IsActive,
                AvailableStock= (int)product.AvailableStock,
                SKU=product.SKU,
            };
            await _dataBaseContext.Products.AddAsync(newProduct,cancellationToken);
            await _dataBaseContext.SaveChangesAsync(cancellationToken) ;
        }

    }
}
