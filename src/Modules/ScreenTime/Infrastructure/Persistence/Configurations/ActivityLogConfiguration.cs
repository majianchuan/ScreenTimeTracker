using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScreenTimeTracker.Modules.ScreenTime.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence.Configurations
{
    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            // 多个对应一个 App
            builder.HasOne(x => x.App)
                .WithMany()
                .HasForeignKey(x => x.AppId)
                .OnDelete(DeleteBehavior.Cascade) // 删 App 时连带删统计记录
                .IsRequired();

            builder.HasIndex(x => x.LoggedAt)
                .IsUnique();
        }
    }
}
