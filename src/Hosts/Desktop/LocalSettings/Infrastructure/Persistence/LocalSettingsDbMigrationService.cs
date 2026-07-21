using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.State;
using System.Globalization;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Infrastructure.Persistence;

public class LocalSettingsDbMigrationService(
    ILogger<LocalSettingsDbMigrationService> logger,
AppSettingsProvider appSettingsState,
    IServiceScopeFactory scopeFactory)
    : IHostedLifecycleService
{
    // 在所有服务启动之前执行
    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Desktop LocalSettingsDbMigrationService is Starting");

        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LocalSettingsDbContext>();

        await EnsureDatabaseDirectoryAsync(context);

        var allMigrations = context.Database.GetMigrations();
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
        bool isNewDatabase = pendingMigrations.Count() == allMigrations.Count();

        await context.Database.MigrateAsync(cancellationToken);

        if (isNewDatabase)
        {
            logger.LogInformation("New database detected, correcting the current language...");
            string osLanguage = CultureInfo.CurrentUICulture.Name;
            if (!AppSettings.SupportedLanguages.Contains(osLanguage))
                osLanguage = "en-US";
            var appSettings = await context.AppSettings.SingleAsync(cancellationToken);
            appSettings.UpdateLanguage(osLanguage);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Language corrected to: {Language}", osLanguage);
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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Initializing AppSettingsStore with baseline values...");

        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LocalSettingsDbContext>();
        var appSettings = await context.AppSettings.SingleAsync(cancellationToken);

        appSettingsState.Initialize(appSettings.DefaultUIOpenMode, appSettings.IsAutoStartEnabled, appSettings.IsSilentStartEnabled, appSettings.Language);

        logger.LogInformation("Memory AppSettingsStore initialized quietly.");
    }

    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}