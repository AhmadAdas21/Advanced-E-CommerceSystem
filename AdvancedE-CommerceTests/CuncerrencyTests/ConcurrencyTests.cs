using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;
using High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedE_CommerceTests.CuncerrencyTests
{
    public class ConcurrencyTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient client;
        private readonly WebApplicationFactory<Program> factory;

        public ConcurrencyTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory;
            client = factory.CreateClient();
        }

        [Fact]
        public async Task ConcurrentOrders_ShouldPreventOverselling()
        {
            using var scope = factory.Services.CreateScope();

            var database = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

            const int initialStock = 25;
            const int concurrentRequests = 100;
            var customer = new Customer
            {
                CustomerName = "Test Customer",
                CustomerEmail = $"{Guid.NewGuid():N}@test.com",
                CustomerPhone = "0591234567",
                IsVip = false
            };
            await database.Customers.AddAsync(customer);
            var product = new Product
            {
                SKU = Guid.NewGuid().ToString("N")[..20],
                ProductName = "Test Product",
                ProductDescription = "Integration Test",
                ProductPrice = 100m,
                AvailableStock = initialStock,
                IsActive = true
            };
            await database.Products.AddAsync(product);
            await database.SaveChangesAsync();
            var tasks = new List<Task<HttpResponseMessage>>();

            for (int i = 0; i < concurrentRequests; i++)
            {
                var order = new CreateOrder
                {
                    CustomerId = customer.CustomerId,
                    RequestId = Guid.NewGuid(),
                    Items = new List<CreateOrderItem>
                     {
                        new CreateOrderItem
                         {
                             SKU = product.SKU,
                             Quantity = 1
                         }
                     }
                };

                tasks.Add(client.PostAsJsonAsync("/api/orders", order));
            }
            var responses = await Task.WhenAll(tasks);
            var successfulOrders = responses.Count(r => r.StatusCode == HttpStatusCode.Created);

            var rejectedOrders = responses.Count(r => r.StatusCode == HttpStatusCode.Conflict);

            Assert.Equal(initialStock, successfulOrders);

            Assert.Equal(concurrentRequests-initialStock, rejectedOrders);
            var productAfterOrders = await database.Products.AsNoTracking().SingleAsync(p => p.SKU == product.SKU);

            Assert.Equal(0, productAfterOrders.AvailableStock);

            var persistedOrdersCount = await database.Orders.CountAsync(o => o.CustomerId == customer.CustomerId);

            Assert.Equal(initialStock, persistedOrdersCount);
        }

        [Fact]
        public async Task ConcurrentOrders_ShouldPreventDuplicateRequestId()
        {
            using var scope = factory.Services.CreateScope();

            var database = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

            const int initialStock = 100;
            const int concurrentRequests = 50;
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
                ProductDescription = "Concurrency Test",
                ProductPrice = 100m,
                AvailableStock = initialStock,
                IsActive = true
            };

            database.Customers.Add(customer);
            database.Products.Add(product);

            await database.SaveChangesAsync();

           
            var requestId = Guid.NewGuid();

            var tasks = new List<Task<HttpResponseMessage>>();

           
            for (int i = 0; i < concurrentRequests; i++)
            {
                var order = new CreateOrder
                {
                    CustomerId = customer.CustomerId,
                    RequestId = requestId,
                    Items = new List<CreateOrderItem>
                    {
                       new CreateOrderItem
                       {
                         SKU = product.SKU,
                         Quantity = 1
                       }
                    }
                };

                tasks.Add(client.PostAsJsonAsync("/api/orders", order));
            }

            var responses = await Task.WhenAll(tasks);

          
            var successfulOrders = responses.Count(r => r.StatusCode == HttpStatusCode.Created);

            var rejectedOrders = responses.Count(r => r.StatusCode == HttpStatusCode.Conflict);

            Assert.Equal(1, successfulOrders);

            Assert.Equal(concurrentRequests - 1, rejectedOrders);

           
            var persistedOrdersCount = await database.Orders .CountAsync(o => o.RequestId == requestId);

            Assert.Equal(1, persistedOrdersCount);

            
            var productAfterOrders = await database.Products.AsNoTracking().SingleAsync(p => p.SKU == product.SKU);

            Assert.Equal(initialStock-1, productAfterOrders.AvailableStock);

        }

        [Fact]
        public async Task ConcurrentCancellation_ShouldRestoreStockOnlyOnce()
        {
            using var scope = factory.Services.CreateScope();

            var database = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
            const int initialStock = 100;
            const int orderQuantity = 10;
            const int concurrentRequests = 50;
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
                ProductDescription = "Concurrency Test",
                ProductPrice = 100m,
                AvailableStock = initialStock,
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
                    Quantity = orderQuantity,
                    SKU = product.SKU
                  }
                }
            };

            var createResponse = await client.PostAsJsonAsync("/api/orders", order);

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var savedOrder = await database.Orders.AsNoTracking().SingleAsync(o => o.RequestId == order.RequestId);
            var productBeforeCancel = await database.Products.AsNoTracking().SingleAsync(p => p.SKU == product.SKU);

            Assert.Equal(initialStock - orderQuantity,productBeforeCancel.AvailableStock);
            var cancelOrder = new CancelOrder
            {
                OrderId = savedOrder.OrderId,
                Status = 2
            };

            var tasks = new List<Task<HttpResponseMessage>>();

            for (int i = 0; i < concurrentRequests; i++)
            {
                tasks.Add(client.PatchAsJsonAsync($"/api/orders?orderId={savedOrder.OrderId}",cancelOrder));
            }

            var responses = await Task.WhenAll(tasks);
            

            foreach (var group in responses.GroupBy(r => r.StatusCode))
            {
                Console.WriteLine($"{group.Key}: {group.Count()}");
            }

            var successfulCancellations = responses.Count(r => r.StatusCode == HttpStatusCode.OK);

            var rejectedCancellations = responses.Count( r => r.StatusCode == HttpStatusCode.Conflict);
            foreach (var response in responses)
            {
                if (response.StatusCode != HttpStatusCode.OK &&response.StatusCode != HttpStatusCode.Conflict)
                {
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
            Assert.Equal(1, successfulCancellations);
            Assert.Equal(concurrentRequests - 1, rejectedCancellations);
            var productAfterCancell = await database.Products.AsNoTracking().SingleAsync(x => x.SKU == product.SKU);
            Assert.Equal(initialStock, productAfterCancell.AvailableStock);
        }

        [Fact]
        public async Task ConcurrentMultiProductOrders_ShouldMaintainStockConsistency()
        {

        }

        [Fact]
        public async Task ConcurrentCacheReads_ShouldReturnConsistentData()
        {

        }
    }
}
