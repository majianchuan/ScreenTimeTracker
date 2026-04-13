using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.AppBehavior.Domain;
using ScreenTimeTracker.Shared.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.AppBehavior.Infrastructure.Persistence;

public class AppBehaviorDbContext(DbContextOptions<AppBehaviorDbContext> options) : ModuleDbContext(options, "AppBehavior")
{
    public DbSet<UserPreferences> UserPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // 自动应用同程序集内所有 IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppBehaviorDbContext).Assembly);
    }
}
