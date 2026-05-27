using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Infrastructure.Persistence.Configurations
{
    public class AppSettingsConfiguration : IEntityTypeConfiguration<AppSettings>
    {
        public void Configure(EntityTypeBuilder<AppSettings> builder)
        {
            builder.HasData(AppSettings.CreateDefault());
        }
    }
}
