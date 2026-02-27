using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Shared.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

public class ScreenTimeDbContext(DbContextOptions<ScreenTimeDbContext> options) : ModuleDbContext(options, "ScreenTime")
{
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<AppHourlyUsage> AppHourlyUsages { get; set; }
    public DbSet<App> Apps { get; set; }
    public DbSet<AppCategory> AppCategories { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // 自动应用同程序集内所有 IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScreenTimeDbContext).Assembly);
    }
}
