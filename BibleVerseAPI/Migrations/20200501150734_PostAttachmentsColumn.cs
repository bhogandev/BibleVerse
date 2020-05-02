﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace BibleVerseAPI.Migrations
{
    public partial class PostAttachmentsColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "Posts");
        }
    }
}
