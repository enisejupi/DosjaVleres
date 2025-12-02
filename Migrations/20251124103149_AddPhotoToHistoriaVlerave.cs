using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KosovaDoganaModerne.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoToHistoriaVlerave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoNdryshimit",
                table: "HistoriaVlerave",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FormatetiPrintimit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmriFormatit = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LlojiModulit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    HtmlTemplate = table.Column<string>(type: "TEXT", nullable: false),
                    CssStyle = table.Column<string>(type: "TEXT", nullable: false),
                    LogoUrl = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LogoPosition = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PaperSize = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    KrijuarNga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    KrijuarMe = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EshteDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    PerditesomMe = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormatetiPrintimit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreferencatPerdoruesit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Perdoruesi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LlojiPreferences = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Vlera = table.Column<string>(type: "TEXT", nullable: false),
                    KrijuarMe = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PerditesomMe = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferencatPerdoruesit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TabelatCustom",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmriTabeles = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Pershkrimi = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Skema = table.Column<string>(type: "TEXT", nullable: false),
                    KrijuarNga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    KrijuarMe = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EshteAktive = table.Column<bool>(type: "INTEGER", nullable: false),
                    PerditesomMe = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabelatCustom", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormatetiPrintimit");

            migrationBuilder.DropTable(
                name: "PreferencatPerdoruesit");

            migrationBuilder.DropTable(
                name: "TabelatCustom");

            migrationBuilder.DropColumn(
                name: "FotoNdryshimit",
                table: "HistoriaVlerave");
        }
    }
}
