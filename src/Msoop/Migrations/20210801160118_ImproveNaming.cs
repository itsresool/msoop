using Microsoft.EntityFrameworkCore.Migrations;

namespace Msoop.Migrations
{
    public partial class ImproveNaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostLimit",
                table: "Subreddits",
                newName: "MaxPostCount");

            migrationBuilder.RenameColumn(
                name: "LinksAgeLimitInDays",
                table: "Sheets",
                newName: "PostAgeLimitInDays");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxPostCount",
                table: "Subreddits",
                newName: "PostLimit");

            migrationBuilder.RenameColumn(
                name: "PostAgeLimitInDays",
                table: "Sheets",
                newName: "LinksAgeLimitInDays");
        }
    }
}
