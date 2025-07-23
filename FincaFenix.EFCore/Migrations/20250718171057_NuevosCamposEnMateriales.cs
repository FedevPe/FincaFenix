using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class NuevosCamposEnMateriales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnidadEmpaque",
                table: "Material");

            migrationBuilder.AddColumn<string>(
                name: "CodigoSAP",
                table: "Material",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Material",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescripcionSAP",
                table: "Material",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoSAP",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "DescripcionSAP",
                table: "Material");

            migrationBuilder.AddColumn<string>(
                name: "UnidadEmpaque",
                table: "Material",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
