using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KosovaDoganaModerne.Migrations
{
    /// <inheritdoc />
    public partial class AddImazhetProduktitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalPrintFormats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LlojiModulit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FormatPrintimiId = table.Column<int>(type: "INTEGER", nullable: false),
                    VendosurNga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DataVendosjes = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Pershkrimi = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    EshteAktiv = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifikuarMe = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifikuarNga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalPrintFormats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlobalPrintFormats_FormatetiPrintimit_FormatPrintimiId",
                        column: x => x.FormatPrintimiId,
                        principalTable: "FormatetiPrintimit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImazhetProduktit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VleraProduktit_Id = table.Column<int>(type: "INTEGER", nullable: false),
                    ShtegimaImazhit = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    EmriOrigjinal = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    LlojiImazhit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Pershkrimi = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RradhaShfaqjes = table.Column<int>(type: "INTEGER", nullable: false),
                    EshteImazhKryesor = table.Column<bool>(type: "INTEGER", nullable: false),
                    MadhesiaBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    NgarkuarNga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NgarkuarMe = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImazhetProduktit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImazhetProduktit_VleratProdukteve_VleraProduktit_Id",
                        column: x => x.VleraProduktit_Id,
                        principalTable: "VleratProdukteve",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrintAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Perdoruesi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LlojiRaportit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FormatiEksportimit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    NumriRekordeve = table.Column<int>(type: "INTEGER", nullable: false),
                    Filtrat = table.Column<string>(type: "TEXT", nullable: true),
                    FormatPrintimiId = table.Column<int>(type: "INTEGER", nullable: true),
                    DataPrintimit = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AdresaIP = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SesioniId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Shenime = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    EshteSuksesshem = table.Column<bool>(type: "INTEGER", nullable: false),
                    MesazhiGabimit = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintAuditLogs_FormatetiPrintimit_FormatPrintimiId",
                        column: x => x.FormatPrintimiId,
                        principalTable: "FormatetiPrintimit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalPrintFormats_EshteAktiv",
                table: "GlobalPrintFormats",
                column: "EshteAktiv");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalPrintFormats_FormatPrintimiId",
                table: "GlobalPrintFormats",
                column: "FormatPrintimiId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalPrintFormats_LlojiModulit",
                table: "GlobalPrintFormats",
                column: "LlojiModulit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImazhetProduktit_EshteImazhKryesor",
                table: "ImazhetProduktit",
                column: "EshteImazhKryesor");

            migrationBuilder.CreateIndex(
                name: "IX_ImazhetProduktit_LlojiImazhit",
                table: "ImazhetProduktit",
                column: "LlojiImazhit");

            migrationBuilder.CreateIndex(
                name: "IX_ImazhetProduktit_VleraProduktit_Id",
                table: "ImazhetProduktit",
                column: "VleraProduktit_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_DataPrintimit",
                table: "PrintAuditLogs",
                column: "DataPrintimit");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_FormatiEksportimit",
                table: "PrintAuditLogs",
                column: "FormatiEksportimit");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_FormatPrintimiId",
                table: "PrintAuditLogs",
                column: "FormatPrintimiId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_LlojiRaportit",
                table: "PrintAuditLogs",
                column: "LlojiRaportit");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_Perdoruesi",
                table: "PrintAuditLogs",
                column: "Perdoruesi");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_SesioniId",
                table: "PrintAuditLogs",
                column: "SesioniId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalPrintFormats");

            migrationBuilder.DropTable(
                name: "ImazhetProduktit");

            migrationBuilder.DropTable(
                name: "PrintAuditLogs");
        }
    }
}
