using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitOfMeasureSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UnidadMedida",
                columns: new[] { "Id", "Descripcion" },
                values: new object[,]
                {
                    { 1, "Kilogramo" },
                    { 2, "Litro" },
                    { 3, "Unidad" },
                    { 4, "Metro" },
                    { 5, "Bolsa" },
                    { 6, "Caja" },
                    { 7, "Gramo" },
                    { 8, "Centímetro cúbico" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UnidadMedida",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UnidadMedida",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UnidadMedida",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UnidadMedida",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UnidadMedida",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "UnidadMedida",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "UnidadMedida",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "UnidadMedida",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
