using Moq;
using Microsoft.AspNetCore.Mvc;
using High_Traffic_E_Commerce_Inventory_Order_System.Controllers;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.ResultPattern;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel;

namespace AdvancedE_CommerceTests.UnitTests
{
    public class OrderControllerTests
    {
        [Fact]
        public async Task SearchOrder_ShouldReturn404_WhenOrderNotFound()
        {
            //Arrange
            var mock = new Mock<IOrderServices>();
            mock.Setup(s => s.SearchOrder( 999, It.IsAny<CancellationToken>())) .ReturnsAsync(
                Result<High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s.GetOrders>
                .Failure(new AppError(ErrorType.NotFound, "ORDER_NOT_FOUND","Order not found")));
            var controller = new OrderController(mock.Object);
            //ACT
            var response = await controller.SearchOrder(999, CancellationToken.None);

            //Assert
            var result = Assert.IsType<NotFoundObjectResult>(response);

            Assert.Equal(404, result.StatusCode);

            mock.Verify(s => s.SearchOrder(999, It.IsAny<CancellationToken>()),Times.Once
            );
        }
        [Fact]
        public void CreateOrder_ShouldReturn201_WhenOrderIsCreated()
        {
            //Arrange
            var mock = new Mock<IOrderServices>();
            var createOrderDto = new High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s.CreateOrder
            {
                CustomerId = 1,
                Items = new List<CreateOrderItem>
                {
                    new CreateOrderItem
                    {
                        Quantity = 1,
                        SKU="pp-205",
                    }
                },

                RequestId=Guid.NewGuid(),
            };


            mock.Setup(s => s.CreateOrder(createOrderDto, It.IsAny<CancellationToken>())).ReturnsAsync(Result<int>.Success(1)); 

            var controller = new OrderController(mock.Object);

            // Act
            var response = controller.CreateOrder(createOrderDto, CancellationToken.None).Result;

            // Assert
            var result = Assert.IsType<ObjectResult>(response);
            Assert.Equal(201, result.StatusCode);

            mock.Verify(s => s.CreateOrder(createOrderDto, It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public void CreateOrder_ShouldReturn400_WhenValidationFails()
        {
            //Arrange
            var mock = new Mock<IOrderServices>();
            var createOrderDto = new High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s.CreateOrder
            {
                CustomerId = 1,
                Items = null,

                RequestId = Guid.NewGuid(),
            };
            mock.Setup(s => s.CreateOrder(createOrderDto, It.IsAny<CancellationToken>())).ReturnsAsync(Result<int>.Failure( new AppError(ErrorType.Validation,"INVALID_ORDER", "Invalid order request." )));
            var controller = new OrderController(mock.Object);
            // Act
            var response = controller.CreateOrder(createOrderDto, CancellationToken.None).Result;
            var result = Assert.IsType<BadRequestObjectResult>(response);
            //Assert
            Assert.Equal(400,result.StatusCode);
            mock.Verify(s => s.CreateOrder(createOrderDto, It.IsAny<CancellationToken>()), Times.Once);




        }
        [Fact]
        public async Task CreateOrder_ShouldReturn409_WhenStockIsInsufficient()
        {
            var mock = new Mock<IOrderServices>();

            var createOrderDto = new CreateOrder
            {
                CustomerId = 1,
                RequestId = Guid.NewGuid(),
                Items = new List<CreateOrderItem>
                 {
                    new CreateOrderItem
                    {
                         SKU = "pp-205",
                         Quantity = 100
                    } 
                }
            };
            mock.Setup(s => s.CreateOrder(createOrderDto,It.IsAny<CancellationToken>())).ReturnsAsync(Result<int>.Failure(
            new AppError(ErrorType.Conflict,"INSUFFICIENT_STOCK","Insufficient stock for product pp-205.")));

            var controller = new OrderController(mock.Object);
            //act
            var response = await controller.CreateOrder(createOrderDto,CancellationToken.None);

            //Arrange
            var result= Assert.IsType<ConflictObjectResult>(response);
            Assert.Equal(409,result.StatusCode);


        }
        [Fact]
        public async Task CancelOrder_ShouldReturn200_WhenCancellationSucceeds()
        {
            //Arrange
            var mock = new Mock<IOrderServices>();
            var cancelOrderDto = new CancelOrder
            {
                OrderId = 1,
                Status = 2
            };

            mock.Setup(s => s.CancelOrder(1,cancelOrderDto, It.IsAny<CancellationToken>())).ReturnsAsync(Result<int>.Success(1));
            var controller = new OrderController(mock.Object);
            //Act
            var response = await controller.CancelOrder(1,cancelOrderDto,CancellationToken.None);
            var result=Assert.IsType<OkObjectResult>(response);
            //Assert
            Assert.Equal(200,result.StatusCode);

        }
        [Fact]
        public async Task CancelOrder_ShouldReturn409_WhenOrderAlreadyCancelled()
        {

        }



    }
}
