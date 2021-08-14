using Microsoft.EntityFrameworkCore.Migrations;

namespace Msoop.Migrations
{
    public partial class SwitchTo7PostAgeLimit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PostAgeLimitInDays",
                table: "Sheets",
                type: "int",
                nullable: false,
                defaultValue: 7,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 14);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PostAgeLimitInDays",
                table: "Sheets",
                type: "int",
                nullable: false,
                defaultValue: 14,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 7);
        }
    }
}
