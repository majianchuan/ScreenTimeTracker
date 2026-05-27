using System.Data;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Infrastructure.Persistence;

public class LocalSettingsDbMigrationService(
    ILogger<LocalSettingsDbMigrationService> logger,
    IServiceScopeFactory scopeFactory) : IHostedLifecycleService
{
    // 在所有服务启动之前执行
    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Desktop LocalSettingsDbMigrationService is Starting");

        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LocalSettingsDbContext>();

        await EnsureDatabaseDirectoryAsync(context);
        await context.Database.MigrateAsync(cancellationToken: cancellationToken);
        await MigrateFromAppBehaviorAsync(context);
    }

    private async Task MigrateFromAppBehaviorAsync(LocalSettingsDbContext context)
    {
        await using var conn = context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT EXISTS(
                SELECT 1
                FROM sqlite_master
                WHERE type='table'
                AND name='AppBehavior_UserPreferences'
            );
        """;

        var result = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        // 旧表存在
        if (result == 1)
        {
            logger.LogInformation("AppBehavior_UserPreferences table exists.");
            // 用旧表数据替换到新表
            await context.Database.ExecuteSqlRawAsync("""
                REPLACE INTO Desktop_LocalSettings_AppSettings
                SELECT Id, 
                       DefaultUIOpenMode, 
                       IsAutoStartEnabled, 
                       IsSilentStartEnabled, 
                       Language, 
                       ShouldDestroyWindowOnClose
                FROM AppBehavior_UserPreferences
            """);
            // 删旧表
            await context.Database.ExecuteSqlRawAsync("""
                DROP TABLE IF EXISTS AppBehavior_UserPreferences;
            """);

            // 删旧迁移历史表
            await context.Database.ExecuteSqlRawAsync("""
                DROP TABLE IF EXISTS __EFMigrationsHistory_AppBehavior;
            """);
        }
    }

    private static async Task EnsureDatabaseDirectoryAsync(LocalSettingsDbContext context)
    {
        var connectionString = context.Database.GetConnectionString();
        if (string.IsNullOrEmpty(connectionString))
            return;
        var builder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connectionString);
        var dbPath = builder.DataSource;

        if (string.IsNullOrEmpty(dbPath)) return;

        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}