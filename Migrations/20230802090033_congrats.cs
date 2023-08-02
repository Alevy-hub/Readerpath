using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readerpath.Migrations
{
    /// <inheritdoc />
    public partial class congrats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CongratsShowed",
                table: "YearChallenges",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CongratsShowed",
                table: "YearChallenges");
        }
    }
}
