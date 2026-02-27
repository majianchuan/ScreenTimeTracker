using ScreenTimeTracker.BuildingBlocks.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScreenTimeTracker.Modules.ScreenTime.Domain;

public class AppHourlyUsage : Entity
{
    public Guid AppId { get; private set; }
    public App? App { get; private set; }
    public DateTime Hour { get; private set; }
    public long DurationMilliseconds { get; private set; } // 数据库存取
    [NotMapped]
    public TimeSpan Duration // 业务使用
    {
        get => TimeSpan.FromMilliseconds(DurationMilliseconds);
        set => DurationMilliseconds = (long)value.TotalMilliseconds;
    }

    // EF Core
    private AppHourlyUsage() { }

    public static AppHourlyUsage Create(Guid trackedAppId, DateTime hour, TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentException("TotalDuration must be greater than zero.", nameof(duration));
        }
        return new AppHourlyUsage()
        {
            Id = Guid.CreateVersion7(),
            AppId = trackedAppId,
            Hour = hour,
            Duration = duration
        };
    }

    public void Accumulate(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Duration must be greater than zero.", nameof(duration));
        }
        Duration += duration;
    }
}
