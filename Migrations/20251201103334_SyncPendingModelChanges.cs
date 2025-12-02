using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KosovaDoganaModerne.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VleraProduktitId",
                table: "ImazhetProduktit",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImazhetProduktit_VleraProduktitId",
                table: "ImazhetProduktit",
                column: "VleraProduktitId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImazhetProduktit_VleratProdukteve_VleraProduktitId",
                table: "ImazhetProduktit",
                column: "VleraProduktitId",
                principalTable: "VleratProdukteve",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImazhetProduktit_VleratProdukteve_VleraProduktitId",
                table: "ImazhetProduktit");

            migrationBuilder.DropIndex(
                name: "IX_ImazhetProduktit_VleraProduktitId",
                table: "ImazhetProduktit");

            migrationBuilder.DropColumn(
                name: "VleraProduktitId",
                table: "ImazhetProduktit");
        }
    }
}
