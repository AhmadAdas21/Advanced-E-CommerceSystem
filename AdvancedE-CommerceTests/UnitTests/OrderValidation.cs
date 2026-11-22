using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;

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

        }
    }
}
