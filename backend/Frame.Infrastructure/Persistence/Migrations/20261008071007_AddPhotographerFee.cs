using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Frame.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotographerFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PhotographerFee",
                table: "Bookings",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotographerFee",
                table: "Bookings");
        }
    }
}
