using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class VariedadesOrganicas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTrabajo_UserEntity_IdUsuario",
                table: "OrdenTrabajo");

            migrationBuilder.DropTable(
                name: "UserEntity");

            migrationBuilder.DropTable(
                name: "RolEntity");

            migrationBuilder.DropIndex(
                name: "IX_OrdenTrabajo_IdUsuario",
                table: "OrdenTrabajo");

            migrationBuilder.AddColumn<bool>(
                name: "Organico",
                table: "VariedadFrutal",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Organico",
                table: "VariedadFrutal");

            migrationBuilder.CreateTable(
                name: "RolEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEntity_RolEntity_RolId",
                        column: x => x.RolId,
                        principalTable: "RolEntity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajo_IdUsuario",
                table: "OrdenTrabajo",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_UserEntity_RolId",
                table: "UserEntity",
                column: "RolId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTrabajo_UserEntity_IdUsuario",
                table: "OrdenTrabajo",
                column: "IdUsuario",
                principalTable: "UserEntity",
                principalColumn: "Id");
        }
    }
}
