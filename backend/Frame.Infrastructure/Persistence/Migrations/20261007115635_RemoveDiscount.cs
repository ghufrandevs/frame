using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Frame.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Bookings_Amounts_Valid",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "MorningHours",
                table: "Bookings");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Bookings_Amounts_Valid",
                table: "Bookings",
                sql: "[Subtotal] > 0 AND [VatAmount] >= 0 AND [TotalAmount] = [Subtotal] + [VatAmount]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Bookings_Amounts_Valid",
                table: "Bookings");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Bookings",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MorningHours",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Bookings_Amounts_Valid",
                table: "Bookings",
                sql: "[Subtotal] > 0 AND [DiscountAmount] >= 0 AND [VatAmount] >= 0 AND [TotalAmount] >= 0");
        }
    }
}
