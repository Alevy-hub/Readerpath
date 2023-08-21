using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Readerpath.Migrations
{
    /// <inheritdoc />
    public partial class TBR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                    
            migrationBuilder.CreateTable(
                name: "TBRBooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci"),
                    TBRId = table.Column<int>(type: "int", nullable: false),
                    LinkedEditionId = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBRBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBRBooks_Editions_LinkedEditionId",
                        column: x => x.LinkedEditionId,
                        principalTable: "Editions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBRBooks_TBRs_TBRId",
                        column: x => x.TBRId,
                        principalTable: "TBRs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");         

            migrationBuilder.CreateIndex(
                name: "IX_TBRBooks_LinkedEditionId",
                table: "TBRBooks",
                column: "LinkedEditionId");

            migrationBuilder.CreateIndex(
                name: "IX_TBRBooks_TBRId",
                table: "TBRBooks",
                column: "TBRId");
         
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BingoFields");

            migrationBuilder.DropTable(
                name: "ChallengeColors");

            migrationBuilder.DropTable(
                name: "MonthBooks");

            migrationBuilder.DropTable(
                name: "TBRBooks");

            migrationBuilder.DropTable(
                name: "UpdatePromptSeen");

            migrationBuilder.DropTable(
                name: "YearBooks");

            migrationBuilder.DropTable(
                name: "YearChallenges");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Bingos");

            migrationBuilder.DropTable(
                name: "TBRs");

            migrationBuilder.DropTable(
                name: "BookActions");

            migrationBuilder.DropTable(
                name: "Editions");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Publishers");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Genres");
        }
    }
}
