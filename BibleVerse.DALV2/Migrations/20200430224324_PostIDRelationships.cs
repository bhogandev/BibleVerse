using Microsoft.EntityFrameworkCore.Migrations;

namespace BibleVerse.DALV2.Migrations
{
    public partial class PostIDRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostID",
                table: "Videos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostID",
                table: "Photos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostID",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "PostID",
                table: "Photos");
        }
    }
}
