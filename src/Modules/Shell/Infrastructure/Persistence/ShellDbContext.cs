using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.Shell.Domain;
using ScreenTimeTracker.Shared.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.Shell.Infrastructure.Persistence;

public class ShellDbContext(DbContextOptions<ShellDbContext> options) : ModuleDbContext(options, "Shell")
{
    public DbSet<UserSettings> UserSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // 自动应用同程序集内所有 IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShellDbContext).Assembly);
    }
}
