using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Empleado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnfermedadPlaga",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnfermedadPlaga", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Finca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finca", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Frutal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Frutal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frutal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maquinaria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Capacidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnidadCapacidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maquinaria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tarea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreArticulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NombreComercial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    UnidadEmpaque = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Material_Categoria_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Empleado_Finca",
                columns: table => new
                {
                    IdEmpleadoFinca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    IdFinca = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleado_Finca", x => x.IdEmpleadoFinca);
                    table.ForeignKey(
                        name: "FK_Empleado_Finca_Empleado_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Empleado_Finca_Finca_IdFinca",
                        column: x => x.IdFinca,
                        principalTable: "Finca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VariedadFrutal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VariedadFruta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdFruta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariedadFrutal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariedadFrutal_Frutal_IdFruta",
                        column: x => x.IdFruta,
                        principalTable: "Frutal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumReceta = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdMaquina = table.Column<int>(type: "int", nullable: false),
                    Dosis = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnidadDosis = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receta_Maquinaria_IdMaquina",
                        column: x => x.IdMaquina,
                        principalTable: "Maquinaria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EnfermedadPlaga_Material",
                columns: table => new
                {
                    IdEnfermedadPlagaMaterial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdMaterial = table.Column<int>(type: "int", nullable: false),
                    IdEnfermedadPlaga = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnfermedadPlaga_Material", x => x.IdEnfermedadPlagaMaterial);
                    table.ForeignKey(
                        name: "FK_EnfermedadPlaga_Material_EnfermedadPlaga_IdEnfermedadPlaga",
                        column: x => x.IdEnfermedadPlaga,
                        principalTable: "EnfermedadPlaga",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnfermedadPlaga_Material_Material_IdMaterial",
                        column: x => x.IdMaterial,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleSectorFinca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFinca = table.Column<int>(type: "int", nullable: false),
                    NombreCuadro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdVariedad = table.Column<int>(type: "int", nullable: false),
                    CantPlantas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleSectorFinca", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleSectorFinca_Finca_IdFinca",
                        column: x => x.IdFinca,
                        principalTable: "Finca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleSectorFinca_VariedadFrutal_IdVariedad",
                        column: x => x.IdVariedad,
                        principalTable: "VariedadFrutal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleReceta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumReceta = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    IdMaterial = table.Column<int>(type: "int", nullable: false),
                    CantRequerida = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnidadCantRequerida = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CantEstimada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnidadCantEstimada = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleReceta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleReceta_Material_IdMaterial",
                        column: x => x.IdMaterial,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleReceta_Receta_NumReceta",
                        column: x => x.NumReceta,
                        principalTable: "Receta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenTrabajo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumOrden = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdReceta = table.Column<int>(type: "int", nullable: false),
                    IdTarea = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2(2)", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2(2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenTrabajo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenTrabajo_Receta_IdReceta",
                        column: x => x.IdReceta,
                        principalTable: "Receta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenTrabajo_Tarea_IdTarea",
                        column: x => x.IdTarea,
                        principalTable: "Tarea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenTrabajo_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenTrabajo_SectorTrabajado",
                columns: table => new
                {
                    IdOrdenTrabajoSectorTrabajado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrdenTrabajo = table.Column<int>(type: "int", nullable: false),
                    IdSector = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenTrabajo_SectorTrabajado", x => x.IdOrdenTrabajoSectorTrabajado);
                    table.ForeignKey(
                        name: "FK_OrdenTrabajo_SectorTrabajado_DetalleSectorFinca_IdSector",
                        column: x => x.IdSector,
                        principalTable: "DetalleSectorFinca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenTrabajo_SectorTrabajado_OrdenTrabajo_IdOrdenTrabajo",
                        column: x => x.IdOrdenTrabajo,
                        principalTable: "OrdenTrabajo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleOrdenTrabajo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrdenTrabajo = table.Column<int>(type: "int", nullable: false),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    SectorWorkedId = table.Column<int>(type: "int", nullable: true),
                    Rendimiento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    HorasTrabajadas = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleOrdenTrabajo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleOrdenTrabajo_Empleado_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleOrdenTrabajo_OrdenTrabajo_IdOrdenTrabajo",
                        column: x => x.IdOrdenTrabajo,
                        principalTable: "OrdenTrabajo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleOrdenTrabajo_OrdenTrabajo_SectorTrabajado_SectorWorkedId",
                        column: x => x.SectorWorkedId,
                        principalTable: "OrdenTrabajo_SectorTrabajado",
                        principalColumn: "IdOrdenTrabajoSectorTrabajado");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrdenTrabajo_IdEmpleado",
                table: "DetalleOrdenTrabajo",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrdenTrabajo_IdOrdenTrabajo",
                table: "DetalleOrdenTrabajo",
                column: "IdOrdenTrabajo");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrdenTrabajo_SectorWorkedId",
                table: "DetalleOrdenTrabajo",
                column: "SectorWorkedId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleReceta_IdMaterial",
                table: "DetalleReceta",
                column: "IdMaterial");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleReceta_NumReceta",
                table: "DetalleReceta",
                column: "NumReceta");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleSectorFinca_IdFinca",
                table: "DetalleSectorFinca",
                column: "IdFinca");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleSectorFinca_IdVariedad",
                table: "DetalleSectorFinca",
                column: "IdVariedad");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_Finca_IdEmpleado",
                table: "Empleado_Finca",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_Finca_IdFinca",
                table: "Empleado_Finca",
                column: "IdFinca");

            migrationBuilder.CreateIndex(
                name: "IX_EnfermedadPlaga_Material_IdEnfermedadPlaga",
                table: "EnfermedadPlaga_Material",
                column: "IdEnfermedadPlaga");

            migrationBuilder.CreateIndex(
                name: "IX_EnfermedadPlaga_Material_IdMaterial",
                table: "EnfermedadPlaga_Material",
                column: "IdMaterial");

            migrationBuilder.CreateIndex(
                name: "IX_Material_IdCategoria",
                table: "Material",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_IdReceta",
                table: "OrdenTrabajo",
                column: "IdReceta");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_IdTarea",
                table: "OrdenTrabajo",
                column: "IdTarea");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_IdUsuario",
                table: "OrdenTrabajo",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_SectorTrabajado_IdOrdenTrabajo",
                table: "OrdenTrabajo_SectorTrabajado",
                column: "IdOrdenTrabajo");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_SectorTrabajado_IdSector",
                table: "OrdenTrabajo_SectorTrabajado",
                column: "IdSector");

            migrationBuilder.CreateIndex(
                name: "IX_Receta_IdMaquina",
                table: "Receta",
                column: "IdMaquina");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_RolId",
                table: "Usuario",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_VariedadFrutal_IdFruta",
                table: "VariedadFrutal",
                column: "IdFruta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleOrdenTrabajo");

            migrationBuilder.DropTable(
                name: "DetalleReceta");

            migrationBuilder.DropTable(
                name: "Empleado_Finca");

            migrationBuilder.DropTable(
                name: "EnfermedadPlaga_Material");

            migrationBuilder.DropTable(
                name: "OrdenTrabajo_SectorTrabajado");

            migrationBuilder.DropTable(
                name: "Empleado");

            migrationBuilder.DropTable(
                name: "EnfermedadPlaga");

            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropTable(
                name: "DetalleSectorFinca");

            migrationBuilder.DropTable(
                name: "OrdenTrabajo");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Finca");

            migrationBuilder.DropTable(
                name: "VariedadFrutal");

            migrationBuilder.DropTable(
                name: "Receta");

            migrationBuilder.DropTable(
                name: "Tarea");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Frutal");

            migrationBuilder.DropTable(
                name: "Maquinaria");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
