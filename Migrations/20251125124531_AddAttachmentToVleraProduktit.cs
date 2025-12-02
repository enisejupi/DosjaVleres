using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KosovaDoganaModerne.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentToVleraProduktit : Migration
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

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "VleratProdukteve",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Bashkangjitje", "EmeriBashkangjitjes" },
                values: new object[] { null, null });
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
