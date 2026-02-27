using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.PatchUserSettings;
using ScreenTimeTracker.Modules.Shell.Infrastructure.OS;
using ScreenTimeTracker.Modules.Shell.Infrastructure.Persistence;
using ScreenTimeTracker.Shared.Infrastructure.Options;

namespace ScreenTimeTracker.Modules.Shell;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShellServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddDbContext<ShellDbContext>((serviceProvider, options) =>
        {
            var persistenceOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseSqlite($"Data Source={persistenceOptions.DBFilePath}", x =>
                x.MigrationsHistoryTable("__EFMigrationsHistory_Shell"));
        });
        services.AddHostedService<ShellDbMigrationService>();

        if (OperatingSystem.IsWindows())
        {
            services.AddSingleton<IStartupManager, WindowsStartupManager>();
        }
        else
        {
            throw new PlatformNotSupportedException("Shell module is only supported on Windows.");
        }

        return services;
    }
}