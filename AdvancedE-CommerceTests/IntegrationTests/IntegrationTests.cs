using System.Text;
using System.Threading.Tasks;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace AdvancedE_CommerceTests.IntegrationTests
{
    public class IntegrationTests
    {
        private readonly DataBaseContext database;
        private readonly OrderServices orderServices;
        private readonly ProductServices productServices;
        private readonly CustomerServices customerServices;
        private readonly HttpClient client;
        private readonly WebApplicationFactory<Program> webApplicationFactory;
        public IntegrationTests(DataBaseContext database, OrderServices orderServices, ProductServices productServices, CustomerServices customerServices, HttpClient client, WebApplicationFactory<Program> webApplicationFactory)
        {
            this.database = database;
            this.orderServices = orderServices;
            this.productServices = productServices;
            this.customerServices = customerServices;
            this.client = client;
            this.webApplicationFactory = webApplicationFactory;
        }


        [Fact]
        public async Task CreateProduct_ShouldReturn201_AndPersistToDatabase()
        {

        }
        [Fact]
        public async Task CreateProduct_ShouldReturn400_WhenProductIsInvalid()
        {

        }
        [Fact]
        public async Task UpdateProduct_ShouldPersistChanges_AndInvalidateCache()
        {

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
