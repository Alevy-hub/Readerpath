using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readerpath.Migrations
{
    /// <inheritdoc />
    public partial class init32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthBooks_Books_BestBookId",
                table: "MonthBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_MonthBooks_Books_WorstBookId",
                table: "MonthBooks");

            migrationBuilder.AddForeignKey(
                name: "FK_MonthBooks_BookActions_BestBookId",
                table: "MonthBooks",
                column: "BestBookId",
                principalTable: "BookActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MonthBooks_BookActions_WorstBookId",
                table: "MonthBooks",
                column: "WorstBookId",
                principalTable: "BookActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthBooks_BookActions_BestBookId",
                table: "MonthBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_MonthBooks_BookActions_WorstBookId",
                table: "MonthBooks");

            migrationBuilder.AddForeignKey(
                name: "FK_MonthBooks_Books_BestBookId",
                table: "MonthBooks",
                column: "BestBookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MonthBooks_Books_WorstBookId",
                table: "MonthBooks",
                column: "WorstBookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
