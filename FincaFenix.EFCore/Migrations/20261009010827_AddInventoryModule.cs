using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostoReferencia",
                table: "Material",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdDivisa",
                table: "Material",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "IdUnidadMedida",
                table: "Material",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Divisa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Simbolo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReservaMaterial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrdenTrabajo = table.Column<int>(type: "int", nullable: false),
                    IdMaterial = table.Column<int>(type: "int", nullable: false),
                    IdFinca = table.Column<int>(type: "int", nullable: false),
                    CantidadReservada = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CantidadConsumida = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false, defaultValue: 0m),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(2)", nullable: false),
                    FechaLiberacion = table.Column<DateTime>(type: "datetime2(2)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservaMaterial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservaMaterial_Finca_IdFinca",
                        column: x => x.IdFinca,
                        principalTable: "Finca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservaMaterial_Material_IdMaterial",
                        column: x => x.IdMaterial,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservaMaterial_OrdenTrabajo_IdOrdenTrabajo",
                        column: x => x.IdOrdenTrabajo,
                        principalTable: "OrdenTrabajo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockPorFinca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdMaterial = table.Column<int>(type: "int", nullable: false),
                    IdFinca = table.Column<int>(type: "int", nullable: false),
                    StockFisico = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false, defaultValue: 0m),
                    StockReservado = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false, defaultValue: 0m),
                    StockMinimo = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false, defaultValue: 0m),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPorFinca", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockPorFinca_Finca_IdFinca",
                        column: x => x.IdFinca,
                        principalTable: "Finca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPorFinca_Material_IdMaterial",
                        column: x => x.IdMaterial,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnidadMedida",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadMedida", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Consumo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrdenTrabajo = table.Column<int>(type: "int", nullable: false),
                    IdMaterial = table.Column<int>(type: "int", nullable: false),
                    CantidadConsumida = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false, defaultValue: 0m),
                    CantidadAplicada = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false, defaultValue: 0m),
                    Unidad = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Origen = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    IdDivisa = table.Column<int>(type: "int", nullable: false),
                    FechaCalculo = table.Column<DateTime>(type: "datetime2(2)", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consumo_Divisa_IdDivisa",
                        column: x => x.IdDivisa,
                        principalTable: "Divisa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consumo_Material_IdMaterial",
                        column: x => x.IdMaterial,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consumo_OrdenTrabajo_IdOrdenTrabajo",
                        column: x => x.IdOrdenTrabajo,
                        principalTable: "OrdenTrabajo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CostoOrdenTrabajo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrdenTrabajo = table.Column<int>(type: "int", nullable: false),
                    IdMaterial = table.Column<int>(type: "int", nullable: false),
                    IdDivisa = table.Column<int>(type: "int", nullable: false),
                    CantidadPlanificada = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CostoTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    FechaCongelado = table.Column<DateTime>(type: "datetime2(2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostoOrdenTrabajo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostoOrdenTrabajo_Divisa_IdDivisa",
                        column: x => x.IdDivisa,
                        principalTable: "Divisa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostoOrdenTrabajo_Material_IdMaterial",
                        column: x => x.IdMaterial,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostoOrdenTrabajo_OrdenTrabajo_IdOrdenTrabajo",
                        column: x => x.IdOrdenTrabajo,
                        principalTable: "OrdenTrabajo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientoInventario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdMaterial = table.Column<int>(type: "int", nullable: false),
                    IdFinca = table.Column<int>(type: "int", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CostoTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    IdDivisa = table.Column<int>(type: "int", nullable: false),
                    StockAnterior = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    StockResultante = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2(2)", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Origen = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdOrdenTrabajo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientoInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientoInventario_Divisa_IdDivisa",
                        column: x => x.IdDivisa,
                        principalTable: "Divisa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientoInventario_Finca_IdFinca",
                        column: x => x.IdFinca,
                        principalTable: "Finca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientoInventario_Material_IdMaterial",
                        column: x => x.IdMaterial,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientoInventario_OrdenTrabajo_IdOrdenTrabajo",
                        column: x => x.IdOrdenTrabajo,
                        principalTable: "OrdenTrabajo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Divisa",
                columns: new[] { "Id", "Codigo", "Nombre", "Simbolo" },
                values: new object[,]
                {
                    { 1, "ARS", "Peso Argentino", "$" },
                    { 2, "USD", "Dólar Estadounidense", "$" },
                    { 3, "EUR", "Euro", "€" },
                    { 4, "JPY", "Yen Japonés", "¥" },
                    { 5, "GBP", "Libra Esterlina", "£" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Material_IdDivisa",
                table: "Material",
                column: "IdDivisa");

            migrationBuilder.CreateIndex(
                name: "IX_Material_IdUnidadMedida",
                table: "Material",
                column: "IdUnidadMedida");

            migrationBuilder.CreateIndex(
                name: "IX_Consumo_IdDivisa",
                table: "Consumo",
                column: "IdDivisa");

            migrationBuilder.CreateIndex(
                name: "IX_Consumo_IdMaterial",
                table: "Consumo",
                column: "IdMaterial");

            migrationBuilder.CreateIndex(
                name: "IX_Consumo_IdOrdenTrabajo_IdMaterial",
                table: "Consumo",
                columns: new[] { "IdOrdenTrabajo", "IdMaterial" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostoOrdenTrabajo_IdDivisa",
                table: "CostoOrdenTrabajo",
                column: "IdDivisa");

            migrationBuilder.CreateIndex(
                name: "IX_CostoOrdenTrabajo_IdMaterial",
                table: "CostoOrdenTrabajo",
                column: "IdMaterial");

            migrationBuilder.CreateIndex(
                name: "IX_CostoOrdenTrabajo_IdOrdenTrabajo_IdMaterial",
                table: "CostoOrdenTrabajo",
                columns: new[] { "IdOrdenTrabajo", "IdMaterial" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Divisa_Codigo",
                table: "Divisa",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_IdDivisa",
                table: "MovimientoInventario",
                column: "IdDivisa");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_IdFinca_Fecha",
                table: "MovimientoInventario",
                columns: new[] { "IdFinca", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_IdMaterial_Fecha",
                table: "MovimientoInventario",
                columns: new[] { "IdMaterial", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_IdOrdenTrabajo",
                table: "MovimientoInventario",
                column: "IdOrdenTrabajo");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaMaterial_IdFinca",
                table: "ReservaMaterial",
                column: "IdFinca");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaMaterial_IdMaterial",
                table: "ReservaMaterial",
                column: "IdMaterial");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaMaterial_IdOrdenTrabajo",
                table: "ReservaMaterial",
                column: "IdOrdenTrabajo");

            migrationBuilder.CreateIndex(
                name: "IX_StockPorFinca_IdFinca",
                table: "StockPorFinca",
                column: "IdFinca");

            migrationBuilder.CreateIndex(
                name: "IX_StockPorFinca_IdMaterial_IdFinca",
                table: "StockPorFinca",
                columns: new[] { "IdMaterial", "IdFinca" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Material_Divisa_IdDivisa",
                table: "Material",
                column: "IdDivisa",
                principalTable: "Divisa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Material_UnidadMedida_IdUnidadMedida",
                table: "Material",
                column: "IdUnidadMedida",
                principalTable: "UnidadMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Material_Divisa_IdDivisa",
                table: "Material");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_UnidadMedida_IdUnidadMedida",
                table: "Material");

            migrationBuilder.DropTable(
                name: "Consumo");

            migrationBuilder.DropTable(
                name: "CostoOrdenTrabajo");

            migrationBuilder.DropTable(
                name: "MovimientoInventario");

            migrationBuilder.DropTable(
                name: "ReservaMaterial");

            migrationBuilder.DropTable(
                name: "StockPorFinca");

            migrationBuilder.DropTable(
                name: "UnidadMedida");

            migrationBuilder.DropTable(
                name: "Divisa");

            migrationBuilder.DropIndex(
                name: "IX_Material_IdDivisa",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_Material_IdUnidadMedida",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "CostoReferencia",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "IdDivisa",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "IdUnidadMedida",
                table: "Material");
        }
    }
}
