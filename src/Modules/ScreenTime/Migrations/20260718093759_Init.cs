using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    IconPath = table.Column<string>(type: "TEXT", nullable: true),
                    IconLastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
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
                    AppIconDirectory = table.Column<string>(type: "TEXT", nullable: false),
                    AppInfoStaleThreshold = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    ActiveSessionAutoSaveInterval = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    IsIdleDetectionEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdleThreshold = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    IdleDetectionPollingInterval = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    MinValidSessionDuration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    SessionMergeTolerance = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    SessionOptimizationInterval = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    DayCutoffHour = table.Column<int>(type: "INTEGER", nullable: false)
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
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessName = table.Column<string>(type: "TEXT", nullable: false),
                    IsAutoUpdateEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastAutoUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AppCategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExecutablePath = table.Column<string>(type: "TEXT", nullable: true),
                    IconPath = table.Column<string>(type: "TEXT", nullable: true),
                    IconLastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false)
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
                name: "ScreenTime_AppUsageSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AppId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsOptimized = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenTime_AppUsageSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreenTime_AppUsageSessions_ScreenTime_Apps_AppId",
                        column: x => x.AppId,
                        principalTable: "ScreenTime_Apps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ScreenTime_AppCategories",
                columns: new[] { "Id", "Color", "IconLastUpdatedAt", "IconPath", "IsSystem", "Name" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "#8E8E93", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, "Uncategorized" });

            migrationBuilder.InsertData(
                table: "ScreenTime_UserSettings",
                columns: new[] { "Id", "ActiveSessionAutoSaveInterval", "AppIconDirectory", "AppInfoStaleThreshold", "DayCutoffHour", "IdleDetectionPollingInterval", "IdleThreshold", "IsIdleDetectionEnabled", "MinValidSessionDuration", "SessionMergeTolerance", "SessionOptimizationInterval" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new TimeSpan(0, 0, 0, 15, 0), "./Data/AppIcons", new TimeSpan(1, 0, 0, 0, 0), 5, new TimeSpan(0, 0, 0, 10, 0), new TimeSpan(0, 0, 10, 0, 0), false, new TimeSpan(0, 0, 0, 5, 0), new TimeSpan(0, 0, 0, 6, 0), new TimeSpan(0, 0, 10, 0, 0) });

            migrationBuilder.InsertData(
                table: "ScreenTime_Apps",
                columns: new[] { "Id", "AppCategoryId", "Color", "ExecutablePath", "IconLastUpdatedAt", "IconPath", "IsAutoUpdateEnabled", "IsSystem", "LastAutoUpdatedAt", "Name", "ProcessName" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001"), "#C7C7CC", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Idle", "Idle" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000001"), "#636366", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unknown", "Unknown" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_AppCategories_Name",
                table: "ScreenTime_AppCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_Apps_AppCategoryId",
                table: "ScreenTime_Apps",
                column: "AppCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_Apps_ProcessName",
                table: "ScreenTime_Apps",
                column: "ProcessName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_AppUsageSessions_AppId_EndTime",
                table: "ScreenTime_AppUsageSessions",
                columns: new[] { "AppId", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_AppUsageSessions_EndTime",
                table: "ScreenTime_AppUsageSessions",
                column: "EndTime");

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTime_AppUsageSessions_StartTime",
                table: "ScreenTime_AppUsageSessions",
                column: "StartTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScreenTime_AppUsageSessions");

            migrationBuilder.DropTable(
                name: "ScreenTime_UserSettings");

            migrationBuilder.DropTable(
                name: "ScreenTime_Apps");

            migrationBuilder.DropTable(
                name: "ScreenTime_AppCategories");
        }
    }
}
