using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkOrderRendimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rendimiento",
                table: "DetalleOrdenTrabajo",
                newName: "Maquinadas");

            migrationBuilder.AddColumn<int>(
                name: "ModoRendimiento",
                table: "Tarea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CantidadProducida",
                table: "DetalleOrdenTrabajo",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModoRendimiento",
                table: "Tarea");

            migrationBuilder.DropColumn(
                name: "CantidadProducida",
                table: "DetalleOrdenTrabajo");

            migrationBuilder.RenameColumn(
                name: "Maquinadas",
                table: "DetalleOrdenTrabajo",
                newName: "Rendimiento");
        }
    }
}
