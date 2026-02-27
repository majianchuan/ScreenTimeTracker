using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ScreenTimeTracker.Modules.ScreenTime.Features.ActivityLogs.RecordActivity;
using ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.AggregateAppUsage;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.OS;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;
using ScreenTimeTracker.Shared.Infrastructure.Options;

namespace ScreenTimeTracker.Modules.ScreenTime;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScreenTimeServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 数据库
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.AddDbContext<ScreenTimeDbContext>((serviceProvider, options) =>
        {
            var persistenceOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

            options.UseSqlite($"Data Source={persistenceOptions.DBFilePath}", x =>
                x.MigrationsHistoryTable("__EFMigrationsHistory_ScreenTime"));
        });
        services.AddHostedService<ScreenTimeDbMigrationService>();

        // 其他服务
        services.AddHostedService<RecordActivityJob>();
        services.AddHostedService<AggregateAppUsageJob>();
        if (OperatingSystem.IsWindowsVersionAtLeast(5, 0))
        {
            services.AddSingleton<IExecutableMetadataProvider, WindowsExecutableMetadataProvider>();
            services.AddSingleton<IForegroundWindowService, WindowsForegroundWindowService>();
            services.AddSingleton<IIdleTimeProvider, WindowsIdleTimeProvider>();
        }

        return services;
    }
}