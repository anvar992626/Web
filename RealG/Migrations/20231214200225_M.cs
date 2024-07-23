using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealG.Migrations
{
    /// <inheritdoc />
    public partial class M : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Titel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Beskrivning = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Längd = table.Column<int>(type: "int", nullable: false),
                    Pris = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salong",
                columns: table => new
                {
                    SalongId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Namn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AntalStolar = table.Column<int>(type: "int", nullable: false),
                    Row = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salong", x => x.SalongId);
                });

            migrationBuilder.CreateTable(
                name: "Föreställning",
                columns: table => new
                {
                    FöreställningId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tider = table.Column<TimeSpan>(type: "time", nullable: false),
                    FilmId = table.Column<int>(type: "int", nullable: false),
                    SalongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Föreställning", x => x.FöreställningId);
                    table.ForeignKey(
                        name: "FK_Föreställning_Movie_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Föreställning_Salong_SalongId",
                        column: x => x.SalongId,
                        principalTable: "Salong",
                        principalColumn: "SalongId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bokning",
                columns: table => new
                {
                    BokningId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    StolNummer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KundNamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KundEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FöreställningId = table.Column<int>(type: "int", nullable: false),
                    BokadDatum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bokning", x => x.BokningId);
                    table.ForeignKey(
                        name: "FK_Bokning_Föreställning_FöreställningId",
                        column: x => x.FöreställningId,
                        principalTable: "Föreställning",
                        principalColumn: "FöreställningId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bokning_FöreställningId",
                table: "Bokning",
                column: "FöreställningId");

            migrationBuilder.CreateIndex(
                name: "IX_Föreställning_FilmId",
                table: "Föreställning",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_Föreställning_SalongId",
                table: "Föreställning",
                column: "SalongId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bokning");

            migrationBuilder.DropTable(
                name: "Föreställning");

            migrationBuilder.DropTable(
                name: "Movie");

            migrationBuilder.DropTable(
                name: "Salong");
        }
    }
}
