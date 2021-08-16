using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Msoop.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sheets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostAgeLimitInDays = table.Column<int>(type: "int", nullable: false, defaultValue: 7),
                    AllowOver18 = table.Column<bool>(type: "bit", nullable: false),
                    AllowSpoilers = table.Column<bool>(type: "bit", nullable: false),
                    AllowStickied = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sheets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subreddits",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SheetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxPostCount = table.Column<int>(type: "int", nullable: false),
                    PostOrdering = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subreddits", x => new { x.SheetId, x.Name });
                    table.ForeignKey(
                        name: "FK_Subreddits_Sheets_SheetId",
                        column: x => x.SheetId,
                        principalTable: "Sheets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subreddits");

            migrationBuilder.DropTable(
                name: "Sheets");
        }
    }
}
