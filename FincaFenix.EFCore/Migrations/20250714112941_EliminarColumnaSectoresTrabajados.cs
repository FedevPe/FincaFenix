using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class EliminarColumnaSectoresTrabajados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTrabajo_SectorTrabajado_OrdenTrabajo_WorkOrderEntityId",
                table: "OrdenTrabajo_SectorTrabajado");

            migrationBuilder.DropIndex(
                name: "IX_OrdenTrabajo_SectorTrabajado_WorkOrderEntityId",
                table: "OrdenTrabajo_SectorTrabajado");

            migrationBuilder.DropColumn(
                name: "WorkOrderEntityId",
                table: "OrdenTrabajo_SectorTrabajado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkOrderEntityId",
                table: "OrdenTrabajo_SectorTrabajado",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_SectorTrabajado_WorkOrderEntityId",
                table: "OrdenTrabajo_SectorTrabajado",
                column: "WorkOrderEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTrabajo_SectorTrabajado_OrdenTrabajo_WorkOrderEntityId",
                table: "OrdenTrabajo_SectorTrabajado",
                column: "WorkOrderEntityId",
                principalTable: "OrdenTrabajo",
                principalColumn: "Id");
        }
    }
}
