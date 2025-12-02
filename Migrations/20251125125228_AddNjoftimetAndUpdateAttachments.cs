using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KosovaDoganaModerne.Migrations
{
    /// <inheritdoc />
    public partial class AddNjoftimetAndUpdateAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Njoftimet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulli = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Pershkrimi = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Lloji = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Linku = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Perdoruesi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EshteLexuar = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataKrijimit = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataLeximit = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ikona = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Njoftimet", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Njoftimet_DataKrijimit",
                table: "Njoftimet",
                column: "DataKrijimit");

            migrationBuilder.CreateIndex(
                name: "IX_Njoftimet_EshteLexuar",
                table: "Njoftimet",
                column: "EshteLexuar");

            migrationBuilder.CreateIndex(
                name: "IX_Njoftimet_Lloji",
                table: "Njoftimet",
                column: "Lloji");

            migrationBuilder.CreateIndex(
                name: "IX_Njoftimet_Perdoruesi",
                table: "Njoftimet",
                column: "Perdoruesi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Njoftimet");
        }
    }
}
