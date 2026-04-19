using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScreenTimeTracker.Modules.AppBehavior.Domain;

namespace ScreenTimeTracker.Modules.AppBehavior.Infrastructure.Persistence.Configurations
{
    public class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferences>
    {
        public void Configure(EntityTypeBuilder<UserPreferences> builder)
        {
            builder.HasData(UserPreferences.CreateDefault());
        }
    }
}
