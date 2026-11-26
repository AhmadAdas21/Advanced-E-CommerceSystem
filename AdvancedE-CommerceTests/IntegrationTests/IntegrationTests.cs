using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;
using High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel;
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
            var singleProductResponse = await client.GetAsync($"/api/product/id/{savedProduct.ProductId}");

            Assert.Equal(HttpStatusCode.OK,singleProductResponse.StatusCode);

            var productCacheKey =$"products:v2:id:{savedProduct.ProductId}";

            Assert.NotNull(await cache.GetStringAsync(productCacheKey));
            var updatedProduct = new UpdateProduct
            {
                SKU = product.SKU,
                ProductName = "Updated Apple Juice",
                ProductDescription = "Updated Description",
                ProductPrice = 200m,
                AvailableStock = 50,
                IsActive = true
            };
            var updateResponse = await client.PutAsJsonAsync( $"/api/product/id/{savedProduct.ProductId}",updatedProduct);

            Assert.Equal(HttpStatusCode.OK,updateResponse.StatusCode);

            var productAfterUpdate = await database.Products.AsNoTracking().SingleAsync(p => p.SKU == product.SKU);
            var cacheAfterUpdate = await cache.GetStringAsync("products:v2:all");

            Assert.Null(cacheAfterUpdate);

        }
        [Fact]
        public async Task CreateCustomer_ShouldPersistCustomerToDatabase()
        {
            using var scope = factory.Services.CreateScope();

            var database = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

            var customer = new Customer
            {
                CustomerName = "Test Customer",
                CustomerEmail = $"{Guid.NewGuid():N}@test.com",
                CustomerPhone = "0591234567",
                IsVip = false
            };
            await database.AddAsync(customer);
            await database.SaveChangesAsync();
            var existcustomer=await database.Customers.AsNoTracking().FirstOrDefaultAsync(x=>x.CustomerId==customer.CustomerId);
            Assert.NotNull(existcustomer);

        }
        [Fact]
        public async Task CreateOrder_ShouldPersistOrder_AndReduceStock()
        {
            using var scope = factory.Services.CreateScope();

            var database = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

            var customer = new Customer
            {
                CustomerName = "Test Customer",
                CustomerEmail = $"{Guid.NewGuid():N}@test.com",
                CustomerPhone = "0591234567",
                IsVip = false
            };
            var product = new Product
            {
                SKU = Guid.NewGuid().ToString("N")[..20],
                ProductName = "Test Product",
                ProductDescription = "Integration Test",
                ProductPrice = 100m,
                AvailableStock = 25,
                IsActive = true
            };

            database.Customers.Add(customer);
            database.Products.Add(product);

            await database.SaveChangesAsync();
            var order = new CreateOrder
            {
                CustomerId=customer.CustomerId,
                RequestId=Guid.NewGuid(),
                Items =new List<CreateOrderItem>
                {
                    new CreateOrderItem
                    {
                        SKU=product.SKU,
                        Quantity=10,
                    }
                }

            };
            var response= await client.PostAsJsonAsync("/api/orders", order);
            var productAfterOrder = await database.Products.AsNoTracking().FirstOrDefaultAsync(x => x.SKU == product.SKU);
            
            Assert.Equal(15, productAfterOrder.AvailableStock);
            
        }
        [Fact]
        public async Task CreateOrder_ShouldReturn409_WhenStockIsInsufficient()
        {
            using var scope = factory.Services.CreateScope();

            var database = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

            var customer = new Customer
            {
                CustomerName = "Test Customer",
                CustomerEmail = $"{Guid.NewGuid():N}@test.com",
                CustomerPhone = "0591234567",
                IsVip = false
            };
            var product = new Product
            {
                SKU = Guid.NewGuid().ToString("N")[..20],
                ProductName = "Test Product",
                ProductDescription = "Integration Test",
                ProductPrice = 100m,
                AvailableStock = 25,
                IsActive = true
            };

            database.Customers.Add(customer);
            database.Products.Add(product);

            await database.SaveChangesAsync();
            var order = new CreateOrder
            {
                CustomerId = customer.CustomerId,
                RequestId = Guid.NewGuid(),
                Items = new List<CreateOrderItem>
                {
                    new CreateOrderItem
                    {
                        SKU = product.SKU,
                        Quantity = 100000,
                    }
                }
            };

            var response = await client.PostAsJsonAsync("/api/orders", order);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode); 
        }
        [Fact]
        public async Task CreateOrder_ShouldReturn404_WhenCustomerNotFound()
        {
            var order = new CreateOrder
            {
                CustomerId=8000,
                RequestId= Guid.NewGuid(),
                Items=new List<CreateOrderItem>
                {
                    new CreateOrderItem
                    {
                        SKU="iou-323-1",
                        Quantity=1,
                    }
                }
            };
            var response =await client.PostAsJsonAsync("/api/orders",order);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task CreateOrder_ShouldPreventDuplicateRequestId()
        {
            using var scope = factory.Services.CreateScope();

            var database = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

            var customer = new Customer
            {
                CustomerName = "Test Customer",
                CustomerEmail = $"{Guid.NewGuid():N}@test.com",
                CustomerPhone = "0591234567",
                IsVip = false
            };
            var product = new Product
            {
                SKU = Guid.NewGuid().ToString("N")[..20],
                ProductName = "Test Product",
                ProductDescription = "Integration Test",
                ProductPrice = 100m,
                AvailableStock = 25,
                IsActive = true
            };

            database.Customers.Add(customer);
            database.Products.Add(product);

            await database.SaveChangesAsync();
            var order1 = new CreateOrder
            {
                CustomerId = customer.CustomerId,
                RequestId = Guid.NewGuid(),
                Items = new List<CreateOrderItem>
                {
                    new CreateOrderItem
                    {
                        SKU = product.SKU,
                        Quantity = 1,
                    }
                }
            };
            var response1 = await client.PostAsJsonAsync("/api/orders", order1);
            var order2 = new CreateOrder
            {
                CustomerId=customer.CustomerId,
                RequestId=order1.RequestId,
                Items=new List<CreateOrderItem>
                {
                    new CreateOrderItem
                    {
                        SKU = product.SKU,
                        Quantity=1,
                    }
                }
            };
            var response2 = await client.PostAsJsonAsync("/api/orders", order2);
            Assert.Equal(HttpStatusCode.Conflict,response2.StatusCode);

            
            
        }
        [Fact]
        public async Task CancelOrder_ShouldRestoreStock_AndUpdateStatus()
        {
            using var scope = factory.Services.CreateScope();

            var database = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

            var customer = new Customer
            {
                CustomerName = "Test Customer",
                CustomerEmail = $"{Guid.NewGuid():N}@test.com",
                CustomerPhone = "0591234567",
                IsVip = false
            };
            var product = new Product
            {
                SKU = Guid.NewGuid().ToString("N")[..20],
                ProductName = "Test Product",
                ProductDescription = "Integration Test",
                ProductPrice = 100m,
                AvailableStock = 25,
                IsActive = true
            };

            database.Customers.Add(customer);
            database.Products.Add(product);

            await database.SaveChangesAsync();
            var order1 = new CreateOrder
            {
                CustomerId = customer.CustomerId,
                RequestId = Guid.NewGuid(),
                Items = new List<CreateOrderItem>
                {
                    new CreateOrderItem
                    {
                        SKU = product.SKU,
                        Quantity = 1,
                    }
                }
            };
            var response1 = await client.PostAsJsonAsync("/api/orders", order1);
            Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
            var productAfterOrder = await database.Products.AsNoTracking().FirstOrDefaultAsync(x=>x.SKU== product.SKU);
            Assert.Equal(24, productAfterOrder.AvailableStock);
            var savedOrder = await database.Orders.AsNoTracking().SingleAsync(x => x.RequestId == order1.RequestId);
            var cancelOrder = new CancelOrder
            {
                OrderId = savedOrder.OrderId,
                Status = 2
            };
            var cancelling = await client.PatchAsJsonAsync($"/api/orders?orderId={savedOrder.OrderId}",cancelOrder);

            Assert.Equal(HttpStatusCode.OK, cancelling.StatusCode);
            var productAfterCanellation=await database.Products.AsNoTracking().FirstOrDefaultAsync(x=>x.SKU== product.SKU);
            Assert.Equal(25, productAfterCanellation.AvailableStock);

        }
        [Fact]
        public async Task GetProducts_ShouldReturnUpdatedData_AfterCacheInvalidation()
        {

        }
    }
}
