using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;


namespace AdvancedE_CommerceTests.IntegrationTests
{
    public class IntegrationTests: IClassFixture<WebApplicationFactory<Program>>
    {
       
        private readonly HttpClient client;
        private readonly WebApplicationFactory<Program> factory;
        public IntegrationTests( WebApplicationFactory<Program> factory)
        {
            this.factory = factory;
            client = factory.CreateClient();
        }


        [Fact]
        public async Task CreateProduct_ShouldReturn201_AndPersistToDatabase()
        {
            var product = new CreateProduct
            {
                IsActive = true,
                SKU = Guid.NewGuid().ToString("N")[..20],
                AvailableStock = 100,
                ProductName = "apple juice",
                ProductDescription = "sd,lsv,ldf,vldfv,lfv,ldf,vldf,vldf,vldf,vld,",
                ProductPrice = 20,
            };
            var response = await client.PostAsJsonAsync("/api/product", product);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode); 
        }
        [Fact]
        public async Task CreateProduct_ShouldReturn400_WhenProductIsInvalid()
        {
            var product = new CreateProduct
            {
                IsActive = true,
                SKU = Guid.NewGuid().ToString("N")[..20],
                AvailableStock = -199,
                ProductName = "apple juice",
                ProductDescription = "sd,lsv,ldf,vldfv,lfv,ldf,vldf,vldf,vldf,vld,",
                ProductPrice = 0,
            };

            var response=await client.PostAsJsonAsync("/api/product", product);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task UpdateProduct_ShouldPersistChanges_AndInvalidateCache()
        {
            var product = new CreateProduct
            {
                SKU = Guid.NewGuid().ToString("N")[..20],
                ProductName = "Apple Juice",
                ProductDescription = "Test Product",
                ProductPrice = 100m,
                AvailableStock = 25,
                IsActive = true
            };
            
            var response= await client.PostAsJsonAsync("/api/product", product);
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            using var scope = factory.Services.CreateScope();

            var database = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

            var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

            var savedProduct = await database.Products.AsNoTracking().SingleAsync(p => p.SKU == product.SKU);
            var getResponse = await client.GetAsync("/api/product");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var cachedData = await cache.GetStringAsync("products:v2:all");

            Assert.NotNull(cachedData);

        }
        [Fact]
        public async Task CreateCustomer_ShouldPersistCustomerToDatabase()
        {

        }
        [Fact]
        public async Task CreateOrder_ShouldPersistOrder_AndReduceStock()
        {

        }
        [Fact]
        public async Task CreateOrder_ShouldReturn409_WhenStockIsInsufficient()
        {

        }
        [Fact]
        public async Task CreateOrder_ShouldReturn404_WhenCustomerNotFound()
        {

        }
        [Fact]
        public async Task CreateOrder_ShouldPreventDuplicateRequestId()
        {

        }
        [Fact]
        public async Task CancelOrder_ShouldRestoreStock_AndUpdateStatus()
        {

        }
        [Fact]
        public async Task GetProducts_ShouldReturnUpdatedData_AfterCacheInvalidation()
        {

        }
    }
}
