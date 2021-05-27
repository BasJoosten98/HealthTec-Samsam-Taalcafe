using Microsoft.EntityFrameworkCore.Migrations;

namespace Taalcafe.Migrations
{
    public partial class AddedHasParticipatedToUserEtry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasParticipated",
                table: "UserEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasParticipated",
                table: "UserEntries");
        }
    }
}
