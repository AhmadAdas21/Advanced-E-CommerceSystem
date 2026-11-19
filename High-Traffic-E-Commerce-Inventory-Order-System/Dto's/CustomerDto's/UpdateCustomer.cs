using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;
using High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s
{
    public record UpdateCustomer
    {
        [Required]
        public string? CustomerName { get; set; }
        [Required]
        public string? CustomerEmail { get; set; }
        [Required]
        public string? CustomerPhone { get; set; }

        [EnumDataType(typeof(NotificationChannel))]
        public NotificationChannel CustomerPrefferdChannel { get; init; }= NotificationChannel.Email;

    }
}
