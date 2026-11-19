using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace High_Traffic_E_Commerce_Inventory_Order_System.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerNotificationPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerPrefferdChannel",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerPrefferdChannel",
                table: "Customers");
        }
    }
}
