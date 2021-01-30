using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BibleVerse.DALV2.Migrations
{
    public partial class RefCodeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefCodeLogs",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationID = table.Column<string>(nullable: true),
                    RefCodeType = table.Column<string>(nullable: true),
                    RefCode = table.Column<string>(nullable: true),
                    isExpired = table.Column<bool>(nullable: false),
                    isUsed = table.Column<bool>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefCodeLogs", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefCodeLogs");
        }
    }
}
