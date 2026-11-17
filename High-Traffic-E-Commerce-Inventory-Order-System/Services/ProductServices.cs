using System.Text.Json;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Services
{
    public class ProductServices:IProductServices
    {
        
        private readonly DataBaseContext _dataBaseContext;
        private readonly IDistributedCache _distributedCache;
        private const string ProductsCacheKey = "products:all";
        public ProductServices(DataBaseContext dataBaseContext,IDistributedCache distributedCache)
        {
            _dataBaseContext = dataBaseContext;
            _distributedCache = distributedCache;
        }
        public async Task<List<GetProducts>> GetAllProducts(CancellationToken cancellationToken)
        {
            var cacheData=await _distributedCache.GetStringAsync(ProductsCacheKey,cancellationToken);
            if (!string.IsNullOrEmpty(cacheData))
            {
                var cachedProducts =JsonSerializer.Deserialize<List<GetProducts>>(cacheData);

                if (cachedProducts != null)
                {
                    return cachedProducts;
                }
            }
            var products = await _dataBaseContext.Products.AsNoTracking().Select(product => new GetProducts
        {
            ProductDescription = product.ProductDescription,
            ProductName = product.ProductName,
            ProductPrice = product.ProductPrice,
            SKU = product.SKU,
            AvailableStock = product.AvailableStock,
            IsActive = product.IsActive}).ToListAsync(cancellationToken);
            await _distributedCache.SetStringAsync(ProductsCacheKey,JsonSerializer.Serialize(products),new DistributedCacheEntryOptions
            { AbsoluteExpirationRelativeToNow =TimeSpan.FromSeconds(15) }, cancellationToken);
            return products;

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

            existProduct.ProductName = product.ProductName;
            existProduct.ProductPrice = product.ProductPrice;
            existProduct.ProductDescription = product.ProductDescription;
            existProduct.IsActive = (bool)product.IsActive;
            existProduct.AvailableStock = (int)product.AvailableStock;
            existProduct.SKU = product.SKU;
            
            
            await _dataBaseContext.SaveChangesAsync(cancellationToken) ;
        }
        public async Task<GetProducts> SearchProductBySKU(string KSU,CancellationToken cancellationToken)
        {
            var product = await _dataBaseContext.Products.FirstOrDefaultAsync(product => product.SKU == KSU);
            if(product == null)
            {
                throw new Exception("the product isnt exist");
            }
            return new GetProducts
            {
                ProductName= product.ProductName,
                ProductPrice= product.ProductPrice,
                ProductDescription= product.ProductDescription,
                IsActive = (bool)product.IsActive,
                AvailableStock = (int)product.AvailableStock,
                SKU = product.SKU,

            };
        }
    }
}
