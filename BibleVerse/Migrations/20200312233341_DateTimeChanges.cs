using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BibleVerse.Migrations
{
    public partial class DateTimeChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    AssignmentId = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Summary = table.Column<string>(nullable: false),
                    ExpPoints = table.Column<int>(nullable: false),
                    CourseId = table.Column<string>(nullable: true),
                    RwdPoints = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Difficulty = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<string>(nullable: true),
                    URL = table.Column<string>(nullable: false),
                    CreatorUsername = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ExprDateTime = table.Column<DateTime>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.AssignmentId);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Difficulty = table.Column<int>(nullable: false),
                    TtlRwdPoints = table.Column<int>(nullable: false),
                    TtlExpPoints = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<string>(nullable: false),
                    CreatorUsername = table.Column<string>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ExprDateTime = table.Column<DateTime>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Body = table.Column<string>(nullable: false),
                    Recipient = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ReadDateTime = table.Column<DateTime>(nullable: false),
                    SentDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    OrganizationId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Members = table.Column<int>(nullable: false),
                    PhoneNum = table.Column<string>(nullable: true),
                    SubsciberId = table.Column<string>(nullable: true),
                    OrgSettingsId = table.Column<string>(nullable: false),
                    IsSuspended = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.OrganizationId);
                });

            migrationBuilder.CreateTable(
                name: "OrgSettings",
                columns: table => new
                {
                    OrgSettingsId = table.Column<string>(nullable: false),
                    OrganizationName = table.Column<string>(nullable: false),
                    MemMsgEnabled = table.Column<bool>(nullable: false),
                    FollowersEnabled = table.Column<bool>(nullable: false),
                    SharingEnabled = table.Column<bool>(nullable: false),
                    OrgMsgEnabled = table.Column<bool>(nullable: false),
                    CalendarSharing = table.Column<bool>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgSettings", x => x.OrgSettingsId);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    PhotoId = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Caption = table.Column<string>(nullable: true),
                    URL = table.Column<string>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.PhotoId);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Body = table.Column<string>(nullable: false),
                    Likes = table.Column<int>(nullable: false),
                    Comments = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    URL = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    ProfileId = table.Column<string>(nullable: false),
                    Picture = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Followers = table.Column<int>(nullable: false),
                    Following = table.Column<int>(nullable: false),
                    Theme = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.ProfileId);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignments",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    AssignmentId = table.Column<string>(nullable: false),
                    OrganizationId = table.Column<string>(nullable: false),
                    AssignmentURL = table.Column<string>(nullable: false),
                    Grade = table.Column<int>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IsCorrected = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignments", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserCourses",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    CourseId = table.Column<string>(nullable: false),
                    OrganizationId = table.Column<string>(nullable: false),
                    OverallGrade = table.Column<int>(nullable: false),
                    IsCorrected = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourses", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    ExpPoints = table.Column<int>(nullable: false),
                    RwdPoints = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    OnlineStatus = table.Column<string>(nullable: true),
                    Friends = table.Column<int>(nullable: false),
                    PhoneNum = table.Column<string>(nullable: true),
                    DOB = table.Column<DateTime>(nullable: false),
                    Age = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<string>(nullable: false),
                    isSuspended = table.Column<bool>(nullable: false),
                    isDeleted = table.Column<bool>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    VideoId = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Caption = table.Column<string>(nullable: true),
                    URL = table.Column<string>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ChangeDateTime = table.Column<DateTime>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.VideoId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "OrgSettings");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "UserAssignments");

            migrationBuilder.DropTable(
                name: "UserCourses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Videos");
        }
    }
}
