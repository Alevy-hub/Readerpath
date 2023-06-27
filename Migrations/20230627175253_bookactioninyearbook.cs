using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readerpath.Migrations
{
    /// <inheritdoc />
    public partial class bookactioninyearbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YearBooks_Books_BestBookId",
                table: "YearBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_YearBooks_Books_WorstBookId",
                table: "YearBooks");

            migrationBuilder.AddForeignKey(
                name: "FK_YearBooks_BookActions_BestBookId",
                table: "YearBooks",
                column: "BestBookId",
                principalTable: "BookActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_YearBooks_BookActions_WorstBookId",
                table: "YearBooks",
                column: "WorstBookId",
                principalTable: "BookActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YearBooks_BookActions_BestBookId",
                table: "YearBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_YearBooks_BookActions_WorstBookId",
                table: "YearBooks");

            migrationBuilder.AddForeignKey(
                name: "FK_YearBooks_Books_BestBookId",
                table: "YearBooks",
                column: "BestBookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_YearBooks_Books_WorstBookId",
                table: "YearBooks",
                column: "WorstBookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
