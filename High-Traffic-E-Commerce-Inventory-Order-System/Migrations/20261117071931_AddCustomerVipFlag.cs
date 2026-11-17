using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace High_Traffic_E_Commerce_Inventory_Order_System.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerVipFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVip",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVip",
                table: "Customers");
        }
    }
}
