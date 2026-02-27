using ScreenTimeTracker.BuildingBlocks.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScreenTimeTracker.Modules.ScreenTime.Domain;

public class ActivityLog : Entity
{
    public Guid AppId { get; private set; }
    public App? App { get; private set; }
    public DateTime LoggedAt { get; private set; }
    public long DurationMilliseconds { get; private set; } // 数据库存取
    [NotMapped]
    public TimeSpan Duration // 业务使用
    {
        get => TimeSpan.FromMilliseconds(DurationMilliseconds);
        set => DurationMilliseconds = (long)value.TotalMilliseconds;
    }

    // EF Core
    private ActivityLog() { }

    public static ActivityLog Create(Guid appId, DateTime loggedAt, TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Duration must be greater than zero.", nameof(duration));
        }
        return new ActivityLog()
        {
            Id = Guid.CreateVersion7(),
            AppId = appId,
            LoggedAt = loggedAt,
            Duration = duration
        };
    }

    public void MarkAsIdle(Guid idleAppId)
    {
        AppId = idleAppId;
    }
}