using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ScreenTimeTracker.Shared.Infrastructure.Options;


namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Infrastructure.Persistence
{
    public class LocalSettingsDbContextFactory : IDesignTimeDbContextFactory<LocalSettingsDbContext>
    {
        public LocalSettingsDbContext CreateDbContext(string[] args)
        {
            var options = new DatabaseOptions();
            var optionsBuilder = new DbContextOptionsBuilder<LocalSettingsDbContext>();

            optionsBuilder.UseSqlite($"Data Source={options.DBFilePath}", x =>
                x.MigrationsHistoryTable("__EFMigrationsHistory_Desktop_LocalSettings"));

            return new LocalSettingsDbContext(optionsBuilder.Options);
        }
    }
}
