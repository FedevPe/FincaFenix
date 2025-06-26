using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class newTablesAndModifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTrabajo_Receta_IdReceta",
                table: "OrdenTrabajo");

            migrationBuilder.DropIndex(
                name: "IX_OrdenTrabajo_IdReceta",
                table: "OrdenTrabajo");

            migrationBuilder.AlterColumn<int>(
                name: "IdReceta",
                table: "OrdenTrabajo",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaFin",
                table: "OrdenTrabajo",
                type: "datetime2(2)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(2)");

            migrationBuilder.CreateTable(
                name: "Correlativo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoDoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Correlativo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_IdReceta",
                table: "OrdenTrabajo",
                column: "IdReceta",
                unique: true,
                filter: "[IdReceta] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTrabajo_Receta_IdReceta",
                table: "OrdenTrabajo",
                column: "IdReceta",
                principalTable: "Receta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTrabajo_Receta_IdReceta",
                table: "OrdenTrabajo");

            migrationBuilder.DropTable(
                name: "Correlativo");

            migrationBuilder.DropIndex(
                name: "IX_OrdenTrabajo_IdReceta",
                table: "OrdenTrabajo");

            migrationBuilder.AlterColumn<int>(
                name: "IdReceta",
                table: "OrdenTrabajo",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaFin",
                table: "OrdenTrabajo",
                type: "datetime2(2)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2(2)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_IdReceta",
                table: "OrdenTrabajo",
                column: "IdReceta");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTrabajo_Receta_IdReceta",
                table: "OrdenTrabajo",
                column: "IdReceta",
                principalTable: "Receta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
