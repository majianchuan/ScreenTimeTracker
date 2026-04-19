using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScreenTimeTracker.Modules.ScreenTime.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence.Configurations
{
    public class AppUsageSessionConfiguration : IEntityTypeConfiguration<AppUsageSession>
    {
        public void Configure(EntityTypeBuilder<AppUsageSession> builder)
        {
            // 多个 AppUsageSession 对应一个 App
            builder.HasOne(x => x.App)
                .WithMany()
                .HasForeignKey(x => x.AppId)
                .OnDelete(DeleteBehavior.Cascade); // 删 App 时连带删 AppUsageSession

            builder.HasIndex(x => x.StartTime);
            builder.HasIndex(x => x.EndTime);
            builder.HasIndex(x => new { x.AppId, x.EndTime }); // 因为都是两集合交集查询，通常 EndTime 比 StartTime 能筛选掉更多数据
        }
    }
}
