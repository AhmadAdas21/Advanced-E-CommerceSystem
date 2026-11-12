using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;
using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;
using High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel;
using Microsoft.EntityFrameworkCore;
namespace High_Traffic_E_Commerce_Inventory_Order_System.DataBase
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; } 
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Customer>().HasIndex(customer=>customer.CustomerId).IsUnique();

            modelBuilder.Entity<Product>().HasIndex(product => product.SKU).IsUnique();

            modelBuilder.Entity<Order>().Property(Order=>Order.TotalAmount).HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<OrderItem>().Property(oi => oi.TotalPrice).HasPrecision(18, 2);

            modelBuilder.Entity<Product>().Property(product=>product.ProductPrice).HasPrecision(18, 2);

        }

    }
}
