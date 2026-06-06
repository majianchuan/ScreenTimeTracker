using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScreenTimeTracker.Modules.ScreenTime.Migrations
{
    /// <inheritdoc />
    public partial class AddAppColorAndAppCategoryColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ScreenTime_Apps",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ScreenTime_AppCategories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ScreenTime_AppCategories",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "Color",
                value: "#8E8E93");

            migrationBuilder.UpdateData(
                table: "ScreenTime_Apps",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "Color",
                value: "#C7C7CC");

            migrationBuilder.UpdateData(
                table: "ScreenTime_Apps",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                column: "Color",
                value: "#636366");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "ScreenTime_Apps");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "ScreenTime_AppCategories");
        }
    }
}
