using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.ResultPattern;
using Moq;

namespace AdvancedE_CommerceTests.UnitTests
{
    public class OrderController
    {
        [Fact]
        public async Task SearchOrder_ShouldReturn404_WhenOrderNotFound()
        {
            //Arrange
            var mock = new Mock<IOrderServices>();
            mock.Setup(s => s.SearchOrder( 999, It.IsAny<CancellationToken>())) .ReturnsAsync(
                Result<High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s.GetOrders>
                .Failure(new AppError(ErrorType.NotFound, "ORDER_NOT_FOUND","Order not found")));
            //ACT
        }
        [Fact]
        public void CreateOrder_ShouldReturn201_WhenOrderIsCreated()
        {
            //Arrange
        }
        [Fact]
        public void CreateOrder_ShouldReturn400_WhenValidationFails()
        {

        }
        
    }
}
