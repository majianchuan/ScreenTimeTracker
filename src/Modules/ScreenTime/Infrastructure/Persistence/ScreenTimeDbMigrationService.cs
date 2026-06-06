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
        await BackfillColorFieldAsync(context, cancellationToken);
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
            AppCategory.Create("Study","#3B82F6","./PrePreparedAppCategoryIcons/study.svg"),
            AppCategory.Create("Work","#6366F1","./PrePreparedAppCategoryIcons/work.svg"),
            AppCategory.Create("Video","#EF4444","./PrePreparedAppCategoryIcons/video.svg"),
            AppCategory.Create("Game","#F97316","./PrePreparedAppCategoryIcons/game.svg"),
            AppCategory.Create("Music","#A855F7","./PrePreparedAppCategoryIcons/music.svg"),
            AppCategory.Create("Relax","#22C55E","./PrePreparedAppCategoryIcons/relax.svg")
        };

        await context.AppCategories.AddRangeAsync(defaults, ct);
        await context.SaveChangesAsync(ct);
    }

    private static async Task BackfillColorFieldAsync(ScreenTimeDbContext context, CancellationToken ct)
    {
        var apps = await context.Apps
            .Where(a => string.IsNullOrEmpty(a.Color))
            .ToListAsync(ct);

        foreach (var app in apps)
        {
            app.UpdateColor(GenerateColor());
        }

        var appCategories = await context.AppCategories
            .Where(a => string.IsNullOrEmpty(a.Color))
            .ToListAsync(ct);
        foreach (var appCategory in appCategories)
        {
            appCategory.UpdateColor(GenerateColor());
        }
        await context.SaveChangesAsync(ct);
    }

    private static string HslToHex(double h, double s, double l)
    {
        h %= 360;
        s /= 100.0;
        l /= 100.0;

        double c = (1 - Math.Abs(2 * l - 1)) * s;
        double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
        double m = l - c / 2;

        double r1 = 0, g1 = 0, b1 = 0;

        if (h < 60)
            (r1, g1, b1) = (c, x, 0);
        else if (h < 120)
            (r1, g1, b1) = (x, c, 0);
        else if (h < 180)
            (r1, g1, b1) = (0, c, x);
        else if (h < 240)
            (r1, g1, b1) = (0, x, c);
        else if (h < 300)
            (r1, g1, b1) = (x, 0, c);
        else
            (r1, g1, b1) = (c, 0, x);

        int r = (int)Math.Round((r1 + m) * 255);
        int g = (int)Math.Round((g1 + m) * 255);
        int b = (int)Math.Round((b1 + m) * 255);

        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static string GenerateColor()
    {
        int h = Random.Shared.Next(360);
        int s = Random.Shared.Next(60, 100);
        int l = Random.Shared.Next(50, 80);
        return HslToHex(h, s, l);
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}