using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPConsole.Migrations
{
    /// <inheritdoc />
    public partial class BookPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Price_EndDate",
                table: "Books",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price_Price",
                table: "Books",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Price_StartDate",
                table: "Books",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price_EndDate",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Price_Price",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Price_StartDate",
                table: "Books");
        }
    }
}
