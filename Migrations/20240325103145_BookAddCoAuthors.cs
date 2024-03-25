using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPConsole.Migrations
{
    /// <inheritdoc />
    public partial class BookAddCoAuthors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoAuthors",
                table: "Books",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoAuthors",
                table: "Books");
        }
    }
}
