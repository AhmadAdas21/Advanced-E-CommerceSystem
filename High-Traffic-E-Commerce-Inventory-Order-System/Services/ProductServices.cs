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
        private const string ProductsCacheKey = "products:v2:all";
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
            List<CachedProduct>? cachedProducts = null;



            Dictionary<int, int>? stockById = null;

           
            try
            {
                var cacheData = await _distributedCache.GetStringAsync( ProductsCacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cacheData))
                {
                    cachedProducts =JsonSerializer.Deserialize<List<CachedProduct>>(cacheData);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Redis read failed.");
            }

            if (cachedProducts == null)
            {
                await _cacheLock.WaitAsync(cancellationToken);

                try
                {
                   
                    try
                    {
                        var cacheData = await _distributedCache.GetStringAsync( ProductsCacheKey, cancellationToken);

                        if (!string.IsNullOrEmpty(cacheData))
                        {
                            cachedProducts =JsonSerializer.Deserialize<List<CachedProduct>>(cacheData);
                        }
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        _logger.LogWarning(ex, "Redis second read failed.");
                    }

                    if (cachedProducts == null)
                    {
                       
                        var dbProducts = await _dataBaseContext.Products
                            .AsNoTracking()
                            .Select(p => new
                            {
                                p.ProductId,
                                p.SKU,
                                p.ProductName,
                                p.ProductDescription,
                                p.ProductPrice,
                                p.IsActive,
                                p.AvailableStock
                            }).ToListAsync(cancellationToken);

                       
                        stockById = dbProducts.ToDictionary(p => p.ProductId, p => p.AvailableStock);

                        
                        cachedProducts = dbProducts.Select(p => new CachedProduct
                        {
                            ProductId = p.ProductId,
                            SKU = p.SKU,
                            ProductName = p.ProductName,
                            ProductDescription = p.ProductDescription,
                            ProductPrice = p.ProductPrice,
                            IsActive = p.IsActive
                        }).ToList();

                        try
                        {
                            await _distributedCache.SetStringAsync(ProductsCacheKey,JsonSerializer.Serialize(cachedProducts),new DistributedCacheEntryOptions
                                {
                                    AbsoluteExpirationRelativeToNow =TimeSpan.FromSeconds(15)
                                },cancellationToken);
                        }
                        catch (Exception ex) when (ex is not OperationCanceledException)
                        {
                            _logger.LogWarning(ex, "Redis write failed.");
                        }
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }
            }

          
            if (stockById == null)
            {
                var stocks = await _dataBaseContext.Products.AsNoTracking().Select(p => new
                    {
                        p.ProductId,
                        p.AvailableStock
                    }).ToListAsync(cancellationToken);

                stockById = stocks.ToDictionary(
                    p => p.ProductId,
                    p => p.AvailableStock);
            }

           
            return cachedProducts.Where(p => stockById.ContainsKey(p.ProductId)).Select(p => new GetProducts
                {
                    SKU = p.SKU,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductPrice = p.ProductPrice,
                    IsActive = p.IsActive,
                    AvailableStock = stockById[p.ProductId]
                }).ToList();
        }


        public async Task<GetProducts>SearchProduct(int productId, CancellationToken cancellationToken)
        {

            string cacheKey = $"products:v2:id:{productId}";

            CachedProduct? cachedProduct = null;
            int? currentStock = null;

          
            try
            {
                var cacheData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cacheData))
                {
                    cachedProduct =JsonSerializer.Deserialize<CachedProduct>(cacheData);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Redis read failed.");
            }

            if (cachedProduct == null)
            {
                await _productCacheLock.WaitAsync(cancellationToken);

                try
                {
                    
                    try
                    {
                        var cacheData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

                        if (!string.IsNullOrEmpty(cacheData))
                        {
                            cachedProduct =JsonSerializer.Deserialize<CachedProduct>(cacheData);
                        }
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        _logger.LogWarning(ex, "Redis second read failed.");
                    }

                    if (cachedProduct == null)
                    {
                     
                        var product = await _dataBaseContext.Products.AsNoTracking().Where(p => p.ProductId == productId).FirstOrDefaultAsync(cancellationToken);

                        if (product == null)
                        {
                            throw new KeyNotFoundException($"Product {productId} not found");
                        }

                        currentStock = product.AvailableStock;

                    
                        cachedProduct = new CachedProduct
                        {
                            ProductId = product.ProductId,
                            SKU = product.SKU,
                            ProductName = product.ProductName,
                            ProductDescription = product.ProductDescription,
                            ProductPrice = product.ProductPrice,
                            IsActive = product.IsActive
                        };

                        try
                        {
                            await _distributedCache.SetStringAsync(cacheKey,JsonSerializer.Serialize(cachedProduct),new DistributedCacheEntryOptions
                                {
                                    AbsoluteExpirationRelativeToNow =TimeSpan.FromSeconds(15)
                                },cancellationToken);
                        }
                        catch (Exception ex) when (ex is not OperationCanceledException)
                        {
                            _logger.LogWarning(ex, "Redis write failed.");
                        }
                    }
                }
                finally
                {
                    _productCacheLock.Release();
                }
            }

          
            if (currentStock == null)
            {
                currentStock = await _dataBaseContext.Products.AsNoTracking().Where(p => p.ProductId == productId).Select(p => (int?)p.AvailableStock).FirstOrDefaultAsync(cancellationToken);
            }

            if (currentStock == null)
            {
                throw new KeyNotFoundException( $"Product {productId} not found");
            }

           
            return new GetProducts
            {
                SKU = cachedProduct.SKU,
                ProductName = cachedProduct.ProductName,
                ProductDescription = cachedProduct.ProductDescription,
                ProductPrice = cachedProduct.ProductPrice,
                IsActive = cachedProduct.IsActive,
                AvailableStock = currentStock.Value
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
            try
            {
                await _distributedCache.RemoveAsync(ProductsCacheKey,CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to invalidate products cache after creation.");
            }

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
            try
            {
                await _distributedCache.RemoveAsync(ProductsCacheKey,CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to invalidate products list after update.");
            }

            try
            {
                await _distributedCache.RemoveAsync($"products:v2:id:{productId}",CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to invalidate updated product {ProductId}.",productId);
            }
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
            try
            {
                await _distributedCache.RemoveAsync(ProductsCacheKey,CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to invalidate products list after deletion.");
            }

            try
            {
                await _distributedCache.RemoveAsync($"products:v2:id:{productId}", CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to invalidate deleted product {ProductId}.",productId);
            }
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
