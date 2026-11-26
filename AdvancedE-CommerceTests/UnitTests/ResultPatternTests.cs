using High_Traffic_E_Commerce_Inventory_Order_System.ResultPattern;

namespace AdvancedE_CommerceTests.UnitTests
{
    public class ResultPatternTests
    {
        [Fact]
        public void Success_ShouldReturnCorrectValue()
        {
            //Arrange
            int orderId = 25;
            //Act
            var result = Result<int>.Success(orderId);
            //Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            

        }
        [Fact]
        public void Failure_ShouldReturnCorrectError()
        {
            var error = new AppError(ErrorType.Conflict,"INSUFFICIENT_STOCK","Not enough stock");

            // Act
            var result = Result<int>.Failure(error);

            // Assert
            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Equal(error, result.Error);

        }
        [Fact]
        public void Success_ShouldThrow_WhenAccessingError()
        {
            //Arrange
            var result= Result<int>.Success(5);
            //Act and assert
            Assert.Throws<InvalidOperationException>(() => _ = result.Error);
        }
        [Fact]
        public void TFailure_ShouldThrow_WhenAccessingValue()
        {
            //Arrange
            var error= new AppError(ErrorType.Validation, "INVALID_ORDER","Invalid order request");
            var result = Result<int>.Failure(error);
            //Act and assert
            Assert.Throws<InvalidOperationException>( () => _ = result.Value);
        }
        [Fact]
        public void Success_ShouldAcceptZeroValue()
        {
            //Arrange
            var number = 0;
            //ACT
            var result=Result<int>.Success(number);
            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Value);
            
        }
    }
}