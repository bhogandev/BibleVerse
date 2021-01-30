using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BibleVerse.DALV2.Migrations
{
    public partial class UserAWSMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bucket",
                table: "Organization",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<string>(nullable: false),
                    RecipientUserID = table.Column<string>(nullable: true),
                    SenderID = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    DirectURL = table.Column<string>(nullable: true),
                    IsUnread = table.Column<bool>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationID);
                });

            migrationBuilder.CreateTable(
                name: "UserAWS",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Bucket = table.Column<string>(nullable: true),
                    PublicDir = table.Column<string>(nullable: true),
                    PrivateDir = table.Column<string>(nullable: true),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAWS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserHistory",
                columns: table => new
                {
                    ActionID = table.Column<string>(nullable: false),
                    UserID = table.Column<string>(nullable: true),
                    ActionType = table.Column<string>(nullable: true),
                    ActionMessage = table.Column<string>(nullable: true),
                    Prev_Value = table.Column<string>(nullable: true),
                    Curr_Value = table.Column<string>(nullable: true),
                    Misc1 = table.Column<string>(nullable: true),
                    Misc2 = table.Column<string>(nullable: true),
                    Misc3 = table.Column<string>(nullable: true),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistory", x => x.ActionID);
                });

            migrationBuilder.CreateTable(
                name: "UserRelationships",
                columns: table => new
                {
                    RelationshipID = table.Column<string>(nullable: false),
                    FirstUser = table.Column<string>(nullable: true),
                    SecondUser = table.Column<string>(nullable: true),
                    RelationshipType = table.Column<string>(nullable: true),
                    FirstUserConfirmed = table.Column<bool>(nullable: false),
                    SecondUserConfirmed = table.Column<bool>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelationships", x => x.RelationshipID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "UserAWS");

            migrationBuilder.DropTable(
                name: "UserHistory");

            migrationBuilder.DropTable(
                name: "UserRelationships");

            migrationBuilder.DropColumn(
                name: "Bucket",
                table: "Organization");
        }
    }
}
