using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ScreenTimeTracker.Shared.Infrastructure.Options;


namespace ScreenTimeTracker.Modules.AppBehavior.Infrastructure.Persistence
{
    public class AppBehaviorDbContextFactory : IDesignTimeDbContextFactory<AppBehaviorDbContext>
    {
        public AppBehaviorDbContext CreateDbContext(string[] args)
        {
            var options = new DatabaseOptions();
            var optionsBuilder = new DbContextOptionsBuilder<AppBehaviorDbContext>();

            optionsBuilder.UseSqlite($"Data Source={options.DBFilePath}", x =>
                x.MigrationsHistoryTable("__EFMigrationsHistory_AppBehavior"));

            return new AppBehaviorDbContext(optionsBuilder.Options);
        }
    }
}
