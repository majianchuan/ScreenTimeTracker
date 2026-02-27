using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScreenTimeTracker.Modules.Shell.Domain;

namespace ScreenTimeTracker.Modules.Shell.Infrastructure.Persistence.Configurations
{
    public class ShellSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
    {
        public void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            builder.HasData(UserSettings.CreateDefault());
        }
    }
}
