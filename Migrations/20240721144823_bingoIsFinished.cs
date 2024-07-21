﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readerpath.Migrations
{
    /// <inheritdoc />
    public partial class bingoIsFinished : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isFinished",
                table: "Bingos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isFinished",
                table: "Bingos");
        }
    }
}
