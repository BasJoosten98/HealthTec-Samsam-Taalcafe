using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Taalcafe.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gebruiker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefoon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Niveau = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruiker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Thema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Beschrijving = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Afbeeldingen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vragen = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Gebruiker_Id = table.Column<int>(type: "int", nullable: false),
                    Gebruikersnaam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Wachtwoord = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Gebruiker_Id);
                    table.ForeignKey(
                        name: "FK_Account_Gebruiker",
                        column: x => x.Gebruiker_Id,
                        principalTable: "Gebruiker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Thema_Id = table.Column<int>(type: "int", nullable: false),
                    Aanmeldingen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Datum = table.Column<DateTime>(type: "datetime", nullable: true),
                    Duur = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessie_Thema",
                        column: x => x.Thema_Id,
                        principalTable: "Thema",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessiePartners",
                columns: table => new
                {
                    Taalcoach_Id = table.Column<int>(type: "int", nullable: false),
                    Cursist_Id = table.Column<int>(type: "int", nullable: false),
                    Sessie_Id = table.Column<int>(type: "int", nullable: false),
                    Feedback_Taalcoach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feedback_Cursist = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cijfer_Taalcoach = table.Column<int>(type: "int", nullable: true),
                    Cijfer_Cursist = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessiePartners", x => new { x.Taalcoach_Id, x.Cursist_Id, x.Sessie_Id });
                    table.ForeignKey(
                        name: "FK_SessiePartners_Gebruiker",
                        column: x => x.Taalcoach_Id,
                        principalTable: "Gebruiker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessiePartners_Gebruiker1",
                        column: x => x.Cursist_Id,
                        principalTable: "Gebruiker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessiePartners_Sessie",
                        column: x => x.Sessie_Id,
                        principalTable: "Sessie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessie_Thema_Id",
                table: "Sessie",
                column: "Thema_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SessiePartners_Cursist_Id",
                table: "SessiePartners",
                column: "Cursist_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SessiePartners_Sessie_Id",
                table: "SessiePartners",
                column: "Sessie_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "SessiePartners");

            migrationBuilder.DropTable(
                name: "Gebruiker");

            migrationBuilder.DropTable(
                name: "Sessie");

            migrationBuilder.DropTable(
                name: "Thema");
        }
    }
}
