using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlassLibraryManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddVerifiedByUserIdToBorrowTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VerifiedByUserId",
                table: "BorrowTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BorrowTransactions_VerifiedByUserId",
                table: "BorrowTransactions",
                column: "VerifiedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowTransactions_Users_VerifiedByUserId",
                table: "BorrowTransactions",
                column: "VerifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowTransactions_Users_VerifiedByUserId",
                table: "BorrowTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BorrowTransactions_VerifiedByUserId",
                table: "BorrowTransactions");

            migrationBuilder.DropColumn(
                name: "VerifiedByUserId",
                table: "BorrowTransactions");
        }
    }
}
