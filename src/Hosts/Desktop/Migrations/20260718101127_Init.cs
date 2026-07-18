using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScreenTimeTracker.Hosts.Desktop.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Desktop_LocalSettings_AppSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DefaultUIOpenMode = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAutoStartEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSilentStartEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desktop_LocalSettings_AppSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Desktop_LocalSettings_AppSettings",
                columns: new[] { "Id", "DefaultUIOpenMode", "IsAutoStartEnabled", "IsSilentStartEnabled", "Language" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), 0, false, false, "en-US" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Desktop_LocalSettings_AppSettings");
        }
    }
}
