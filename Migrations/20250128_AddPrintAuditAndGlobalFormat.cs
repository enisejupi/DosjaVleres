using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KosovaDoganaModerne.Migrations
{
    /// <summary>
    /// Migrimi për shtimin e auditimit të printimit dhe formateve globale të printimit.
    /// Këto tabela janë kritike për sigurinë dhe kontrollin e operacioneve të printimit.
    /// </summary>
    public partial class AddPrintAuditAndGlobalFormat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Krijimi i tabelës PrintAuditLogs
            migrationBuilder.CreateTable(
                name: "PrintAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Perdoruesi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LlojiRaportit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormatiEksportimit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumriRekordeve = table.Column<int>(type: "int", nullable: false),
                    Filtrat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormatPrintimiId = table.Column<int>(type: "int", nullable: true),
                    DataPrintimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdresaIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SesioniId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Shenime = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EshteSuksesshem = table.Column<bool>(type: "bit", nullable: false),
                    MesazhiGabimit = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
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

            // Krijimi i tabelës GlobalPrintFormats
            migrationBuilder.CreateTable(
                name: "GlobalPrintFormats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlojiModulit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormatPrintimiId = table.Column<int>(type: "int", nullable: false),
                    VendosurNga = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataVendosjes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Pershkrimi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EshteAktiv = table.Column<bool>(type: "bit", nullable: false),
                    ModifikuarMe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifikuarNga = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
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

            // Krijimi i indeksave për PrintAuditLogs
            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_Perdoruesi",
                table: "PrintAuditLogs",
                column: "Perdoruesi");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_LlojiRaportit",
                table: "PrintAuditLogs",
                column: "LlojiRaportit");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_FormatiEksportimit",
                table: "PrintAuditLogs",
                column: "FormatiEksportimit");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_DataPrintimit",
                table: "PrintAuditLogs",
                column: "DataPrintimit");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_SesioniId",
                table: "PrintAuditLogs",
                column: "SesioniId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintAuditLogs_FormatPrintimiId",
                table: "PrintAuditLogs",
                column: "FormatPrintimiId");

            // Krijimi i indeksave për GlobalPrintFormats
            migrationBuilder.CreateIndex(
                name: "IX_GlobalPrintFormats_LlojiModulit",
                table: "GlobalPrintFormats",
                column: "LlojiModulit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalPrintFormats_EshteAktiv",
                table: "GlobalPrintFormats",
                column: "EshteAktiv");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalPrintFormats_FormatPrintimiId",
                table: "GlobalPrintFormats",
                column: "FormatPrintimiId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrintAuditLogs");

            migrationBuilder.DropTable(
                name: "GlobalPrintFormats");
        }
    }
}
