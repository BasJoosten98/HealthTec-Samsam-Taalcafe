using Microsoft.EntityFrameworkCore.Migrations;

namespace Taalcafe.Migrations
{
    public partial class RemovedDescriptionAndQuestionsFromTheme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Themes");

            migrationBuilder.DropColumn(
                name: "Questions",
                table: "Themes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Themes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Questions",
                table: "Themes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
