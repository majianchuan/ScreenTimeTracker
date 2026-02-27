using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ScreenTimeTracker.Shared.Infrastructure.Options;


namespace ScreenTimeTracker.Modules.Shell.Infrastructure.Persistence
{
    public class ShellDbContextFactory : IDesignTimeDbContextFactory<ShellDbContext>
    {
        public ShellDbContext CreateDbContext(string[] args)
        {
            var options = new DatabaseOptions();
            var optionsBuilder = new DbContextOptionsBuilder<ShellDbContext>();

            optionsBuilder.UseSqlite($"Data Source={options.DBFilePath}", x =>
                x.MigrationsHistoryTable("__EFMigrationsHistory_Shell"));

            return new ShellDbContext(optionsBuilder.Options);
        }
    }
}
