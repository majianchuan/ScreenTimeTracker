using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScreenTimeTracker.Modules.ScreenTime.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence.Configurations
{
    public class TrackedAppConfiguration : IEntityTypeConfiguration<App>
    {
        public void Configure(EntityTypeBuilder<App> builder)
        {
            builder.HasOne(a => a.AppCategory)
                .WithMany()
                .HasForeignKey(a => a.AppCategoryId)
                .OnDelete(DeleteBehavior.Restrict) //阻止删除Category，如果存在App。
                .IsRequired();

            builder.HasIndex(x => x.ProcessName)
                .IsUnique();
            builder.HasIndex(x => x.AppCategoryId);
        }
    }
}
