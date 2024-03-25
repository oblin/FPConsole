﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPConsole.Migrations
{
    /// <inheritdoc />
    public partial class BookAddComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Comments",
                table: "Books",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Books");
        }
    }
}
