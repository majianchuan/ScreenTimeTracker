using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScreenTimeTracker.Modules.ScreenTime.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScreenTime_AppCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IconPath = table.Column<string>(type: "TEXT", nullable: true),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenTime_AppCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScreenTime_UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SamplingInterval = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    IdleDetection = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdleTimeout = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    AppInfoStaleThreshold = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    AggregationInterval = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    AppIconDirectory = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenTime_UserSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScreenTime_Apps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessName = table.Column<string>(type: "TEXT", nullable: false),
                    IsAutoUpdateEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastAutoUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AppCategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExecutablePath = table.Column<string>(type: "TEXT", nullable: true),
                    IconPath = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenTime_Apps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreenTime_Apps_ScreenTime_AppCategories_AppCategoryId",
                        column: x => x.AppCategoryId,
                        principalTable: "ScreenTime_AppCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScreenTime_ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AppId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMilliseconds = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenTime_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreenTime_ActivityLogs_ScreenTime_Apps_AppId",
                        column: x => x.AppId,
                        principalTable: "ScreenTime_Apps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScreenTime_AppHourlyUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AppId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Hour = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMilliseconds = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenTime_AppHourlyUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreenTime_AppHourlyUsages_ScreenTime_Apps_AppId",
                        column: x => x.AppId,
                        principalTable: "ScreenTime_Apps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ScreenTime_AppCategories",
                columns: new[] { "Id", "IconPath", "IsSystem", "Name" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), null, true, "Uncategorized" });

            migrationBuilder.InsertData(
                table: "ScreenTime_UserSettings",
                columns: new[] { "Id", "AggregationInterval", "AppIconDirectory", "AppInfoStaleThreshold", "IdleDetection", "IdleTimeout", "SamplingInterval" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new TimeSpan(0, 1, 0, 0, 0), "./Data/Icons", new TimeSpan(1, 0, 0, 0, 0), false, new TimeSpan(0, 0, 10, 0, 0), new TimeSpan(0, 0, 0, 1, 0) });

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_ActivityLogs_AppId",
                table: "ScreenTime_ActivityLogs",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_ActivityLogs_LoggedAt",
                table: "ScreenTime_ActivityLogs",
                column: "LoggedAt",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_AppHourlyUsages_AppId_Hour",
                table: "ScreenTime_AppHourlyUsages",
                columns: new[] { "AppId", "Hour" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_AppHourlyUsages_Hour",
                table: "ScreenTime_AppHourlyUsages",
                column: "Hour");

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_Apps_AppCategoryId",
                table: "ScreenTime_Apps",
                column: "AppCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_Apps_ProcessName",
                table: "ScreenTime_Apps",
                column: "ProcessName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScreenTime_ActivityLogs");

            migrationBuilder.DropTable(
                name: "ScreenTime_AppHourlyUsages");

            migrationBuilder.DropTable(
                name: "ScreenTime_UserSettings");

            migrationBuilder.DropTable(
                name: "ScreenTime_Apps");

            migrationBuilder.DropTable(
                name: "ScreenTime_AppCategories");
        }
    }
}
