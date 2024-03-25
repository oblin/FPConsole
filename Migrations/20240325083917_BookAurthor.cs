using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPConsole.Migrations
{
    /// <inheritdoc />
    public partial class BookAurthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Aurthor_Email",
                table: "Books",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Aurthor_Name",
                table: "Books",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aurthor_Email",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Aurthor_Name",
                table: "Books");
        }
    }
}
