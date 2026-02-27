using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScreenTimeTracker.Modules.Shell.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shell_UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UIOpenMode = table.Column<int>(type: "INTEGER", nullable: false),
                    AutoStart = table.Column<bool>(type: "INTEGER", nullable: false),
                    SilentStart = table.Column<bool>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    WindowDestroyOnClose = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shell_UserSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Shell_UserSettings",
                columns: new[] { "Id", "AutoStart", "Language", "SilentStart", "UIOpenMode", "WindowDestroyOnClose" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), false, "en-US", false, 0, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shell_UserSettings");
        }
    }
}
