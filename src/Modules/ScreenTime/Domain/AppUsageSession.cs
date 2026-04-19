using ScreenTimeTracker.BuildingBlocks.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Domain;

public class AppUsageSession : Entity
{
    public Guid AppId { get; private set; }
    public App? App { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public bool IsOptimized { get; private set; }

    // EF Core
    private AppUsageSession() { }

    public static AppUsageSession Create(Guid appId, DateTime startTime, DateTime endTime)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be greater than start time.", nameof(endTime));
        return new AppUsageSession()
        {
            Id = Guid.CreateVersion7(),
            AppId = appId,
            StartTime = startTime,
            EndTime = endTime,
            IsOptimized = false
        };
    }

    public void MarkAsIdle(Guid idleAppId) => AppId = idleAppId;

    public void UpdateEndTime(DateTime endTime)
    {
        if (endTime <= StartTime)
            throw new ArgumentException("End time must be greater than start time.", nameof(endTime));
        EndTime = endTime;
    }

    public void MarkAsOptimized() => IsOptimized = true;
}
