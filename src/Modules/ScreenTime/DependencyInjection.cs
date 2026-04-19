using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Features.Tracking.TrackActiveSession;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.OS;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.State;
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

        // 后台服务
        services.AddHostedService<ActiveSessionTracker>();
        services.AddHostedService<ActiveSessionAutoSaver>();
        services.AddHostedService<AppUsageSessionOptimizer>();

        // 状态存储
        services.AddSingleton<IActiveSessionStore, InMemoryActiveSessionStore>();

        // 平台
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 0, 6000))
        {
            services.AddSingleton<IForegroundWindowMonitor, WindowsForegroundWindowMonitor>();
            services.AddSingleton<IExecutableMetadataProvider, WindowsExecutableMetadataProvider>();
            services.AddSingleton<IIdleTimeProvider, WindowsIdleTimeProvider>();
        }
        else
        {
            throw new PlatformNotSupportedException("Only Windows XP RTM or later is supported.");
        }

        return services;
    }
}