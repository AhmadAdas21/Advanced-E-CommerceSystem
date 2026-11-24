using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace AdvancedE_CommerceTests.UnitTests
{
    public class RedisServices
    {
        [Fact]
        public async Task GetAllProducts_ShouldReadFromCache_WhenCacheHit()
        {
            var options = new DbContextOptionsBuilder<DataBaseContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using var db = new DataBaseContext(options);

            db.Products.Add(new Product
            {
                ProductId = 1,
                SKU = "pp-205",
                ProductName = " Name1",
                ProductDescription = "Test product",
                ProductPrice = 100m,
                AvailableStock = 25,
                IsActive = true
            });

            await db.SaveChangesAsync();
            var cachedProducts = new List<CachedProduct>
        {
            new CachedProduct
            {
                ProductId = 1,
                SKU = "pp-205",
                ProductName = "Name2",
                ProductDescription = "Test product",
                ProductPrice = 100m,
                IsActive = true
            }
        };

            var mockCache = new Mock<IDistributedCache>();

            mockCache.Setup(c => c.GetAsync("products:v2:all",It.IsAny<CancellationToken>())).ReturnsAsync( JsonSerializer.SerializeToUtf8Bytes(cachedProducts));

            var service = new ProductServices(
                db,
                mockCache.Object,
                Mock.Of<ILogger<ProductServices>>());

            // Act
            var result = await service.GetAllProducts(CancellationToken.None);

            // Assert
            var product = Assert.Single(result);

            Assert.Equal("Name2", product.ProductName);
            Assert.Equal(25, product.AvailableStock);

            mockCache.Verify(c => c.GetAsync("products:v2:all", It.IsAny<CancellationToken>()),Times.Once);
        }
        [Fact]
        public async Task GetAllProducts_ShouldPopulateCache_WhenCacheMiss()
        {
            var mock =new Mock<IDistributedCache>();
            var options = new DbContextOptionsBuilder<DataBaseContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using var db = new DataBaseContext(options);
            db.Products.Add(new Product
            {
                ProductId = 1,
                SKU = "pp-205",
                ProductName = " Name1",
                ProductDescription = "Test product",
                ProductPrice = 100m,
                AvailableStock = 25,
                IsActive = true
            });
            await db.SaveChangesAsync();
            var cachedProducts = new List<CachedProduct>
               {
                   new CachedProduct
                      {
               
                      }
               };
            mock.Setup(c => c.GetAsync("products:v2:all",It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);

            var service = new ProductServices(db,mock.Object,Mock.Of<ILogger<ProductServices>>());

            // Act
            var result = await service.GetAllProducts(CancellationToken.None);
            var product = Assert.Single(result);

            Assert.Equal(" Name1", product.ProductName);
            Assert.Equal(25, product.AvailableStock);

            mock.Verify(c => c.GetAsync("products:v2:all", It.IsAny<CancellationToken>()), Times.Exactly(2));



        }
        [Fact]
        public async Task GetAllProducts_ShouldFallbackToDatabase_WhenRedisUnavailable()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataBaseContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            using var db = new DataBaseContext(options);

            db.Products.Add(new Product
            {
                ProductId = 1,
                SKU = "pp-205",
                ProductName = "Database Product",
                ProductPrice = 100m,
                ProductDescription = "Test product",
                AvailableStock = 25,
                IsActive = true
            });
            await db.SaveChangesAsync();
            var mock= new Mock<IDistributedCache>();

            mock.Setup(c => c.GetAsync("products:v2:all",It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Redis unavailable"));
            mock.Setup(c => c.SetAsync("products:v2:all",It.IsAny<byte[]>(),It.IsAny<DistributedCacheEntryOptions>(),It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Redis unavailable"));
            var service = new ProductServices(db,mock.Object,Mock.Of<ILogger<ProductServices>>());

            var result = await service.GetAllProducts(CancellationToken.None);
            var product = Assert.Single(result);

            Assert.Equal("Database Product", product.ProductName);
            Assert.Equal(25, product.AvailableStock);

            mock.Verify(c => c.GetAsync("products:v2:all",It.IsAny<CancellationToken>()),Times.Exactly(2));
        }
        [Fact]
        public async Task CreateProduct_ShouldInvalidateCache_AfterSuccessfulCreation()
        {

        }
        
    }
}
