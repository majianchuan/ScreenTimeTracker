using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.PatchAppSettings;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Infrastructure.OS;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Infrastructure.Persistence;
using ScreenTimeTracker.Shared.Infrastructure.Options;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLocalSettingsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddDbContext<LocalSettingsDbContext>((serviceProvider, options) =>
        {
            var persistenceOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseSqlite($"Data Source={persistenceOptions.DBFilePath}", x =>
                x.MigrationsHistoryTable("__EFMigrationsHistory_Desktop_LocalSettings"));
        });
        services.AddHostedService<LocalSettingsDbMigrationService>();

        if (OperatingSystem.IsWindows())
        {
            services.AddSingleton<IStartupManager, WindowsStartupManager>();
        }
        else
        {
            throw new PlatformNotSupportedException("Desktop LocalSettings module is only supported on Windows.");
        }

        return services;
    }
}