using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

public class ScreenTimeDbMigrationService(
    ILogger<ScreenTimeDbMigrationService> logger,
    IServiceScopeFactory scopeFactory
    ) : IHostedLifecycleService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<ScreenTimeDbMigrationService> _logger = logger;

    // 在所有服务启动之前执行
    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ScreenTimeDbMigrationService is Starting");
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ScreenTimeDbContext>();
        // 确保数据库目录存在
        var connectionString = context.Database.GetConnectionString();
        var dbPath = GetDatabasePath(connectionString);

        if (!string.IsNullOrEmpty(dbPath))
        {
            var directory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            bool dbFileExists = File.Exists(dbPath);

            // 应用数据库迁移
            await context.Database.MigrateAsync(cancellationToken: cancellationToken);

            // 注入种子数据，数据库文件不存在，则判定为第一次运行
            if (!dbFileExists)
                await SeedDefaultCategoriesAsync(context, cancellationToken);
        }
    }

    private static async Task SeedDefaultCategoriesAsync(ScreenTimeDbContext context, CancellationToken ct)
    {
        var defaults = new List<AppCategory>
        {
            AppCategory.Create("Study","./PrePreparedAppCategoryIcons/study.svg"),
            AppCategory.Create("Work","./PrePreparedAppCategoryIcons/work.svg"),
            AppCategory.Create("Video","./PrePreparedAppCategoryIcons/video.svg"),
            AppCategory.Create("Game","./PrePreparedAppCategoryIcons/game.svg"),
            AppCategory.Create("Music","./PrePreparedAppCategoryIcons/music.svg"),
            AppCategory.Create("Relax","./PrePreparedAppCategoryIcons/coffee.svg")
        };

        await context.AppCategories.AddRangeAsync(defaults, ct);
        await context.SaveChangesAsync(ct);
    }

    private static string? GetDatabasePath(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return null;

        var builder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connectionString);
        return builder.DataSource;
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}