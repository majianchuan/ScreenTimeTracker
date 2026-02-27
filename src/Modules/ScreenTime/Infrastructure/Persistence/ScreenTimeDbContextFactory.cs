using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ScreenTimeTracker.Shared.Infrastructure.Options;


namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence
{
    public class ScreenTimeDbContextFactory : IDesignTimeDbContextFactory<ScreenTimeDbContext>
    {
        public ScreenTimeDbContext CreateDbContext(string[] args)
        {
            var options = new DatabaseOptions();
            var optionsBuilder = new DbContextOptionsBuilder<ScreenTimeDbContext>();

            optionsBuilder.UseSqlite($"Data Source={options.DBFilePath}", x =>
                x.MigrationsHistoryTable("__EFMigrationsHistory_ScreenTime"));

            return new ScreenTimeDbContext(optionsBuilder.Options);
        }
    }
}
