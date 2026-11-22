using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine;
using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Strategies;
using Moq;

namespace AdvancedE_CommerceTests.UnitTests
{
    public class PricingStrategyTests
    {
        [Fact]
        public void NormalDiscount_ShouldReturnOriginalPrice()
        {
            //Arrange
            var normal = new NormalDiscount();
            var originalPrice = 100M;
            //Act
            var result=normal.PayAmount(originalPrice);

            //Assert
            Assert.Equal(originalPrice, result);
        }
        [Fact]
        public void VipDiscount_ShouldApply15PercentDiscount()
        {
            //Arrange
            var vip = new VipDiscount();
            var originalPrice = 100M;
            //act
            var result= vip.PayAmount(originalPrice);
            //Assert
            Assert.Equal(85M, result);
        }
        [Fact]
        public void SeasonalDiscount_ShouldApply20PercentDiscount()
        {
            var originalPrice = 100m;
            //Arrange
            var seasonal = new SeasonalDiscount();
            //Act
            var result=seasonal.PayAmount(originalPrice);
            //Assert
            Assert.Equal(80m,result);

        }
        [Fact]
        public void BlackFridayDiscount_ShouldApply40PercentDiscount()
        {
            var priginalPrice = 100m;
            //Arrange
            var black=new BlackFridayDiscount();
            //Act
            var result=black.PayAmount(priginalPrice);
            //Assert
            Assert.Equal(60m, result);
        }
        [Fact]
        public void PricingEngine_ShouldCallStrategyOnce()
        {
            //Arrange
            var mockStrategy = new Mock<IPricingPlanStrategy>();
            mockStrategy.Setup(s => s.PayAmount(100m)).Returns(85m);
            var engine = new PricingEngine();

            // Act
            decimal result = engine.CalculatePrice(mockStrategy.Object, 100m);
            //Assert
            Assert.Equal(85M, result);
            mockStrategy.Verify(s => s.PayAmount(100m),Times.Once);
        }

    }
}
