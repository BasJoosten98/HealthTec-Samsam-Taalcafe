using Microsoft.EntityFrameworkCore.Migrations;

namespace Taalcafe.Migrations
{
    public partial class RemovedMarkFromUserEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mark",
                table: "UserEntries");

            migrationBuilder.DropColumn(
                name: "MarkReason",
                table: "UserEntries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Mark",
                table: "UserEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MarkReason",
                table: "UserEntries",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
