using ScreenTimeTracker.BuildingBlocks.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Domain;

public class UserSettings : Entity
{
    public static readonly Guid DefaultId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public TimeSpan SamplingInterval { get; private set; }
    public bool IdleDetection { get; private set; }
    public TimeSpan IdleTimeout { get; private set; }
    public TimeSpan AppInfoStaleThreshold { get; private set; }
    public TimeSpan AggregationInterval { get; private set; }
    public string AppIconDirectory { get; private set; }

    // EF Core 构造函数
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private UserSettings() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static UserSettings CreateDefault() => new()
    {
        Id = DefaultId,
        SamplingInterval = TimeSpan.FromSeconds(1),
        IdleDetection = false,
        IdleTimeout = TimeSpan.FromMinutes(10),
        AppInfoStaleThreshold = TimeSpan.FromHours(24),
        AggregationInterval = TimeSpan.FromHours(1),
        AppIconDirectory = "./Data/Icons",
    };

    public void UpdateSamplingInterval(TimeSpan newInterval)
    {
        if (newInterval <= TimeSpan.Zero)
            throw new ArgumentException("Polling interval must be greater than zero.", nameof(newInterval));
        SamplingInterval = newInterval;

    }

    public void UpdateIdleDetection(bool enable) => IdleDetection = enable;

    public void UpdateIdleTimeout(TimeSpan newTimeout)
    {
        if (newTimeout <= TimeSpan.Zero)
            throw new ArgumentException("Idle timeout must be greater than zero.", nameof(newTimeout));
        IdleTimeout = newTimeout;
    }

    public void UpdateAppInfoStaleThreshold(TimeSpan newThreshold)
    {
        if (newThreshold <= TimeSpan.Zero)
            throw new ArgumentException("App info stale threshold must be greater than zero.", nameof(newThreshold));
        AppInfoStaleThreshold = newThreshold;
    }

    public void UpdateAggregationInterval(TimeSpan newInterval)
    {
        if (newInterval <= TimeSpan.Zero)
            throw new ArgumentException("Aggregation interval must be greater than zero.", nameof(newInterval));
        AggregationInterval = newInterval;
    }

    public void UpdateAppIconDirectory(string newDirectory) => AppIconDirectory = newDirectory;
}