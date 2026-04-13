using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ScreenTimeTracker.Modules.AppBehavior.Infrastructure.Persistence;

public class AppBehaviorDbMigrationService(ILogger<AppBehaviorDbMigrationService> logger, IServiceScopeFactory scopeFactory) : IHostedLifecycleService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<AppBehaviorDbMigrationService> _logger = logger;

    // 在所有服务启动之前执行
    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("AppBehaviorDbMigrationService is Starting");
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppBehaviorDbContext>();
        // 确保数据库目录存在
        var connectionString = context.Database.GetConnectionString();
        var dbPath = GetDatabasePath(connectionString);

        if (!string.IsNullOrEmpty(dbPath))
        {
            var directory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await context.Database.MigrateAsync(cancellationToken: cancellationToken);
        }
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