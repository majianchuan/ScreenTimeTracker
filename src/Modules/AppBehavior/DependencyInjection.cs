using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.PatchUserPreferences;
using ScreenTimeTracker.Modules.AppBehavior.Infrastructure.OS;
using ScreenTimeTracker.Modules.AppBehavior.Infrastructure.Persistence;
using ScreenTimeTracker.Shared.Infrastructure.Options;

namespace ScreenTimeTracker.Modules.AppBehavior;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppBehaviorServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddDbContext<AppBehaviorDbContext>((serviceProvider, options) =>
        {
            var persistenceOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseSqlite($"Data Source={persistenceOptions.DBFilePath}", x =>
                x.MigrationsHistoryTable("__EFMigrationsHistory_AppBehavior"));
        });
        services.AddHostedService<AppBehaviorDbMigrationService>();

        if (OperatingSystem.IsWindows())
        {
            services.AddSingleton<IStartupManager, WindowsStartupManager>();
        }
        else
        {
            throw new PlatformNotSupportedException("AppBehavior module is only supported on Windows.");
        }

        return services;
    }
}