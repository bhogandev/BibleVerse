using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BibleVerse.DALV2.Migrations
{
    public partial class AlterUserRelationshipsTble2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRelationships");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRelationships",
                columns: table => new
                {
                    RelationshipID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChangeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstUserConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    RelationshipType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondUserConfirmed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelationships", x => x.RelationshipID);
                });
        }
    }
}
