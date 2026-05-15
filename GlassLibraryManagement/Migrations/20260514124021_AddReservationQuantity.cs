using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlassLibraryManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Reservations");
        }
    }
}
