using Microsoft.EntityFrameworkCore.Migrations;

namespace BibleVerse.DALV2.Migrations
{
    public partial class RefreshTokensUserColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "RefreshTokens",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UsersId",
                table: "RefreshTokens",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UsersId",
                table: "RefreshTokens",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UsersId",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UsersId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "RefreshTokens");
        }
    }
}
