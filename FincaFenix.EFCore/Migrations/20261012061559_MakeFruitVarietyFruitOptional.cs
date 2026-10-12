using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaFenix.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class MakeFruitVarietyFruitOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VariedadFrutal_Frutal_IdFruta",
                table: "VariedadFrutal");

            migrationBuilder.AlterColumn<int>(
                name: "IdFruta",
                table: "VariedadFrutal",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_VariedadFrutal_Frutal_IdFruta",
                table: "VariedadFrutal",
                column: "IdFruta",
                principalTable: "Frutal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VariedadFrutal_Frutal_IdFruta",
                table: "VariedadFrutal");

            migrationBuilder.AlterColumn<int>(
                name: "IdFruta",
                table: "VariedadFrutal",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VariedadFrutal_Frutal_IdFruta",
                table: "VariedadFrutal",
                column: "IdFruta",
                principalTable: "Frutal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
