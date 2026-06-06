using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScreenTimeTracker.Modules.ScreenTime.Migrations
{
    /// <inheritdoc />
    public partial class DelAppDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ScreenTime_Apps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ScreenTime_Apps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ScreenTime_Apps",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "Description",
                value: "Shows when the user is idle");

            migrationBuilder.UpdateData(
                table: "ScreenTime_Apps",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                column: "Description",
                value: "Represents unknown or restricted apps");
        }
    }
}
