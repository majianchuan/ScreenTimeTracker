using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.BuildingBlocks.Infrastructure.Interceptors;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Infrastructure.Persistence.Configurations;
using ScreenTimeTracker.Shared.Infrastructure.Persistence;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Infrastructure.Persistence;

public class LocalSettingsDbContext(
    DbContextOptions<LocalSettingsDbContext> options)
    : ModuleDbContext(options, "Desktop_LocalSettings")
{
    public DbSet<AppSettings> AppSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new AppSettingsConfiguration());

    }
}
