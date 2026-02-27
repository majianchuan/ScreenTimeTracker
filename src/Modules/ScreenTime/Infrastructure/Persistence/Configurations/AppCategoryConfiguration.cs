using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScreenTimeTracker.Modules.ScreenTime.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence.Configurations
{
    public class AppCategoryConfiguration : IEntityTypeConfiguration<AppCategory>
    {
        public void Configure(EntityTypeBuilder<AppCategory> builder)
        {
            builder.HasData(AppCategory.CreateUncategorized());
        }
    }
}
