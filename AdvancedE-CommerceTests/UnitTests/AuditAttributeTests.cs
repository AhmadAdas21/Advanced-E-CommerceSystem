using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using High_Traffic_E_Commerce_Inventory_Order_System.Auditing;
using High_Traffic_E_Commerce_Inventory_Order_System.Controllers;

namespace AdvancedE_CommerceTests.UnitTests
{
    public  class AuditAttributeTests
    {
        [Fact]
        public void CreateOrder_ShouldHaveAuditableAttribute()
        {
            //Arrange and Act
            var customAttribute = Attribute.GetCustomAttribute(typeof(OrderController).GetMethod("CreateOrder")!, typeof(AuditableAttribute));
            //Assert
            Assert.NotNull(customAttribute);

        }
        [Fact]
        public void CancelOrder_ShouldHaveAuditableAttribute()
        {
            var customAttribute = Attribute.GetCustomAttribute(typeof(OrderController).GetMethod("CancelOrder")!, typeof(AuditableAttribute));
            //Assert
            Assert.NotNull(customAttribute);

        }
        [Fact]
        public void GetOrders_ShouldNotHaveAuditableAttribute()
        {

            var customAttribute = Attribute.GetCustomAttribute(typeof(OrderController).GetMethod("GetOrders")!, typeof(AuditableAttribute));
            //Assert
            Assert.Null(customAttribute);

        }
    }
}
