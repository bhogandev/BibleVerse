using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BibleVerse.DALV2.Migrations
{
    public partial class SiteConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteConfigs",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Service = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Misc1 = table.Column<string>(nullable: true),
                    Misc2 = table.Column<string>(nullable: true),
                    Misc3 = table.Column<string>(nullable: true),
                    Misc4 = table.Column<string>(nullable: true),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteConfigs", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteConfigs");
        }
    }
}
