using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

public class ScreenTimeDbMigrationService(
    ILogger<ScreenTimeDbMigrationService> logger,
    IServiceScopeFactory scopeFactory) : IHostedLifecycleService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<ScreenTimeDbMigrationService> _logger = logger;

    // 在所有服务启动之前执行
    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ScreenTimeDbMigrationService is Starting");

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ScreenTimeDbContext>();

        await EnsureDatabaseDirectoryAsync(context);

        var appliedMigrations = await context.Database.GetAppliedMigrationsAsync(cancellationToken);
        bool isNewDatabase = !appliedMigrations.Any();

        await context.Database.MigrateAsync(cancellationToken: cancellationToken);

        if (isNewDatabase)
        {
            _logger.LogInformation("New database detected, seeding default categories...");
            await SeedDefaultCategoriesAsync(context, cancellationToken);
        }

    }

    private static async Task EnsureDatabaseDirectoryAsync(ScreenTimeDbContext context)
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

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}