using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class MakeMaterialUnitOfMeasureRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill defensivo: materiales sin unidad => Unidad (id 3)
            migrationBuilder.Sql("UPDATE [Material] SET [IdUnidadMedida] = 3 WHERE [IdUnidadMedida] IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "IdUnidadMedida",
                table: "Material",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdUnidadMedida",
                table: "Material",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
