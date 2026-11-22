using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using Xunit.Abstractions;

namespace AdvancedE_CommerceTests.UnitTests
{
    public class OrderValidation
    {
        private readonly DataBaseContext _context;
        private readonly IOrderServices orderServices;
        [Fact]
        public void CreateOrder_ShouldFail_WhenOrderItemsAreNull()
        {
            //Arrange
            var order = new CreateOrder
            {
                CustomerId = 1,
                RequestId = Guid.NewGuid(),
                Items = null
            };

            var results = new List<ValidationResult>();
            //Act

            bool isValid = Validator.TryValidateObject(order,new ValidationContext(order),results,validateAllProperties: true );



            //Assert
            Assert.False(isValid);

            Assert.Contains(results, result => result.MemberNames.Contains(nameof(CreateOrder.Items)));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void CreateOrder_ShouldFail_WhenItemQuantityIsInvalid(int quantity)
        {
            //Arrange
            var item = new CreateOrderItem
            {
                SKU = "TEST-001",
                Quantity = quantity
            };
            var results = new List<ValidationResult>();

            //Act

            bool isValid = Validator.TryValidateObject(item,new ValidationContext(item),results,validateAllProperties: true);


            //Assert
            Assert.False(isValid);

            Assert.Contains(results, result =>result.MemberNames.Contains(nameof(CreateOrderItem.Quantity)));
        }
        [Fact]
        public void CreateOrder_ShouldFail_WhenDuplicateSKUsExist()
        {
            //Arrange
            var order = new CreateOrder
            {
                CustomerId = 1,
                RequestId = Guid.NewGuid(),
                Items = new List<CreateOrderItem>
                {
                    new CreateOrderItem
                    {
                        SKU = "PRODUCT-001",
                        Quantity = 2
                    },
                    new CreateOrderItem
                    {
                        SKU = "product-001",
                        Quantity = 3
                    }
                }
            };

            //Act
            bool hasDuplicates = order.Items.GroupBy(
                    item => item.SKU,
                    StringComparer.OrdinalIgnoreCase).Any(group => group.Count() > 1);

            //Assert
            Assert.True(hasDuplicates);
        }
    }
}
