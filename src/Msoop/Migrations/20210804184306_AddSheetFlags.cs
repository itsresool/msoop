using Microsoft.EntityFrameworkCore.Migrations;

namespace Msoop.Migrations
{
    public partial class AddSheetFlags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowOver18",
                table: "Sheets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSpoilers",
                table: "Sheets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowStickied",
                table: "Sheets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowOver18",
                table: "Sheets");

            migrationBuilder.DropColumn(
                name: "AllowSpoilers",
                table: "Sheets");

            migrationBuilder.DropColumn(
                name: "AllowStickied",
                table: "Sheets");
        }
    }
}
