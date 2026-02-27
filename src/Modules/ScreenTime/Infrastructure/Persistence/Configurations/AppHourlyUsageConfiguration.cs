using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScreenTimeTracker.Modules.ScreenTime.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence.Configurations
{
    public class AppHourlyUsageConfiguration : IEntityTypeConfiguration<AppHourlyUsage>
    {
        public void Configure(EntityTypeBuilder<AppHourlyUsage> builder)
        {
            builder.HasOne(x => x.App)
                .WithMany()
                .HasForeignKey(x => x.AppId)
                .OnDelete(DeleteBehavior.Cascade) // 删 App 时连带删统计记录
                .IsRequired();

            builder.HasIndex(x => new { x.AppId, x.Hour })
                .IsUnique();

            builder.HasIndex(x => x.Hour);
        }
    }
}
