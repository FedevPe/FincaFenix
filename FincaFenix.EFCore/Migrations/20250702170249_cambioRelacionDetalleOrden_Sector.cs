using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class cambioRelacionDetalleOrden_Sector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleOrdenTrabajo_OrdenTrabajo_SectorTrabajado_IdSectorTrabajado",
                table: "DetalleOrdenTrabajo");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleOrdenTrabajo_DetalleSectorFinca_IdSectorTrabajado",
                table: "DetalleOrdenTrabajo",
                column: "IdSectorTrabajado",
                principalTable: "DetalleSectorFinca",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleOrdenTrabajo_DetalleSectorFinca_IdSectorTrabajado",
                table: "DetalleOrdenTrabajo");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleOrdenTrabajo_OrdenTrabajo_SectorTrabajado_IdSectorTrabajado",
                table: "DetalleOrdenTrabajo",
                column: "IdSectorTrabajado",
                principalTable: "OrdenTrabajo_SectorTrabajado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
