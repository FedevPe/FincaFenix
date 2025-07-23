using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class NuevColumnaTRVenReceta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnidadDosis",
                table: "Receta",
                newName: "UnidadVa");

            migrationBuilder.RenameColumn(
                name: "Dosis",
                table: "Receta",
                newName: "VolumenAplicacion");

            migrationBuilder.AddColumn<decimal>(
                name: "TRV",
                table: "Receta",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TRV",
                table: "Receta");

            migrationBuilder.RenameColumn(
                name: "VolumenAplicacion",
                table: "Receta",
                newName: "Dosis");

            migrationBuilder.RenameColumn(
                name: "UnidadVa",
                table: "Receta",
                newName: "UnidadDosis");
        }
    }
}
