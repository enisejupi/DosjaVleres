using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KosovaDoganaModerne.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bashkangjitje",
                table: "VleratProdukteve",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmeriBashkangjitjes",
                table: "VleratProdukteve",
                type: "TEXT",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bashkangjitje",
                table: "VleratProdukteve");

            migrationBuilder.DropColumn(
                name: "EmeriBashkangjitjes",
                table: "VleratProdukteve");
        }
    }
}
