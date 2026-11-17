using System.Text.Json;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;


namespace High_Traffic_E_Commerce_Inventory_Order_System.Services
{
    public class ProductServices:IProductServices
    {
        
        private readonly DataBaseContext _dataBaseContext;
        private readonly IDistributedCache _distributedCache;
        private const string ProductsCacheKey = "products:all";
        private static readonly SemaphoreSlim _cacheLock = new(1, 1);
        private static readonly SemaphoreSlim _productCacheLock = new(1, 1);
        private readonly ILogger<ProductServices> _logger;
        public ProductServices(DataBaseContext dataBaseContext,IDistributedCache distributedCache, ILogger<ProductServices> logger)
        {
            _dataBaseContext = dataBaseContext;
            _distributedCache = distributedCache;
            _logger = logger;
        }
        public async Task<List<GetProducts>> GetAllProducts(CancellationToken cancellationToken)
        {
            string? cacheData = null;

            try
            {
                cacheData = await _distributedCache.GetStringAsync(ProductsCacheKey, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Redis read failed. Using SQL Server.");
            }
            if (!string.IsNullOrEmpty(cacheData))
            {
                var cachedProducts =JsonSerializer.Deserialize<List<GetProducts>>(cacheData);

                if (cachedProducts != null)
                {
                    return cachedProducts;
                }
            }
            await _cacheLock.WaitAsync(cancellationToken);
            try
            {

                try
                {
                    cacheData = await _distributedCache.GetStringAsync(ProductsCacheKey, cancellationToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    cacheData = null;
                    _logger.LogWarning(ex, "Redis second read failed.");
                }

                if (!string.IsNullOrEmpty(cacheData))
                {
                    var cachedProducts = JsonSerializer.Deserialize<List<GetProducts>>(cacheData);

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
                      IsActive = product.IsActive
                     }).ToListAsync(cancellationToken);

                try
                {
                    await _distributedCache.SetStringAsync( ProductsCacheKey, JsonSerializer.Serialize(products),
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15)
                        },cancellationToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogWarning(ex, "Redis write failed.");
                }

                return products;

            }
            finally
            {
                _cacheLock.Release();
            }
            

        }
        
        public async Task<GetProducts>SearchProduct(int productId, CancellationToken cancellationToken)
        {
           
            string cacheKey = $"products:id:{productId}";
            string? dataCache = null;

            try
            {
                dataCache = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Redis read failed. Using SQL Server.");
            }
            if (!string.IsNullOrEmpty(dataCache))
            {
                var cachedProduct= JsonSerializer.Deserialize<GetProducts>(dataCache);
                if (cachedProduct != null)
                {
                    return cachedProduct;
                }
            }
            await _productCacheLock.WaitAsync(cancellationToken);
            try
            {

                try
                {
                    dataCache = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    dataCache = null;
                    _logger.LogWarning(ex, "Redis second read failed.");
                }

                if (!string.IsNullOrEmpty(dataCache))
                {
                    var cachedProduct = JsonSerializer.Deserialize<GetProducts>(dataCache);

                    if (cachedProduct != null)
                    {
                        return cachedProduct;
                    }
                }

                var product = await _dataBaseContext.Products.AsNoTracking().Where(p => p.ProductId == productId).Select(p => new GetProducts
                    {
                        ProductDescription = p.ProductDescription,
                        ProductName = p.ProductName,
                        ProductPrice = p.ProductPrice,
                        SKU = p.SKU,
                        AvailableStock = p.AvailableStock,
                        IsActive = p.IsActive
                    }).FirstOrDefaultAsync(cancellationToken);

                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {productId} not found");
                }


                try
                {
                    await _distributedCache.SetStringAsync( cacheKey, JsonSerializer.Serialize(product),
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow =TimeSpan.FromSeconds(15)
                        },cancellationToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogWarning(ex, "Redis write failed.");
                }

                return product;
            }
            finally
            {
               
                _productCacheLock.Release();
            }
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
            await _distributedCache.RemoveAsync(ProductsCacheKey,cancellationToken);

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
            await _distributedCache.RemoveAsync(ProductsCacheKey,cancellationToken);
            await _distributedCache.RemoveAsync($"products:id:{productId}", cancellationToken);
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
            await _distributedCache.RemoveAsync(ProductsCacheKey, cancellationToken);
            await _distributedCache.RemoveAsync($"products:id:{productId}",cancellationToken);
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
