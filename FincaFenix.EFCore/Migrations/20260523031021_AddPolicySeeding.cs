using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicySeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Edad",
                table: "DetalleSectorFinca",
                type: "int",
                nullable: false,
                defaultValue: 2026,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2025);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Edad",
                table: "DetalleSectorFinca",
                type: "int",
                nullable: false,
                defaultValue: 2025,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2026);
        }
    }
}
