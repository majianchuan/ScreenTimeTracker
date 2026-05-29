using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScreenTimeTracker.Modules.ScreenTime.Migrations
{
    /// <inheritdoc />
    public partial class AddAppCategoryNameUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_AppCategories_Name",
                table: "ScreenTime_AppCategories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScreenTime_AppCategories_Name",
                table: "ScreenTime_AppCategories");
        }
    }
}
