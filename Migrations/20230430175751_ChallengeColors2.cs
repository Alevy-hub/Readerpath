using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Readerpath.Migrations
{
    /// <inheritdoc />
    public partial class ChallengeColors2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChallengeColors",
                schema: "ReaderPathDb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ColorForFive = table.Column<string>(type: "longtext", nullable: true),
                    ColorForFour = table.Column<string>(type: "longtext", nullable: true),
                    ColorForThree = table.Column<string>(type: "longtext", nullable: true),
                    ColorForTwo = table.Column<string>(type: "longtext", nullable: true),
                    ColorForOne = table.Column<string>(type: "longtext", nullable: true),
                    ColorForNoRating = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeColors", x => x.Id);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChallengeColors",
                schema: "ReaderPathDb");
        }
    }
}
