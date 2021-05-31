using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Taalcafe.Migrations
{
    public partial class ChangedHasParticipatedToJoined_atInUserEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasParticipated",
                table: "UserEntries");

            migrationBuilder.AddColumn<DateTime>(
                name: "Joined_at",
                table: "UserEntries",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Joined_at",
                table: "UserEntries");

            migrationBuilder.AddColumn<bool>(
                name: "HasParticipated",
                table: "UserEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
