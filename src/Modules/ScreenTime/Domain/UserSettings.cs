using System.ComponentModel.DataAnnotations;
using ScreenTimeTracker.BuildingBlocks.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Domain;

public class UserSettings : Entity
{
    public static readonly Guid DefaultId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    // 记录使用数据
    public string AppIconDirectory { get; private set; }
    public TimeSpan AppInfoStaleThreshold { get; private set; }
    public TimeSpan ActiveSessionAutoSaveInterval { get; private set; }

    // 空闲检测
    public bool IsIdleDetectionEnabled { get; private set; }
    public TimeSpan IdleThreshold { get; private set; }
    public TimeSpan IdleDetectionPollingInterval { get; private set; }

    // 使用数据优化
    public TimeSpan MinValidSessionDuration { get; private set; }
    public TimeSpan SessionMergeTolerance { get; private set; }
    public TimeSpan SessionOptimizationInterval { get; private set; }

    // 数据呈现
    public int DayCutoffHour { get; private set; }

    // EF Core 构造函数
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private UserSettings() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static UserSettings CreateDefault() => new()
    {
        Id = DefaultId,
        AppIconDirectory = "./Data/Icons",
        AppInfoStaleThreshold = TimeSpan.FromHours(24),
        ActiveSessionAutoSaveInterval = TimeSpan.FromSeconds(10),
        IsIdleDetectionEnabled = false,
        IdleThreshold = TimeSpan.FromMinutes(10),
        IdleDetectionPollingInterval = TimeSpan.FromSeconds(10),
        MinValidSessionDuration = TimeSpan.FromSeconds(3),
        SessionMergeTolerance = TimeSpan.FromSeconds(6),
        SessionOptimizationInterval = TimeSpan.FromMinutes(10),
        DayCutoffHour = 5,
    };

    public void UpdateAppIconDirectory(string appIconDirectory) => AppIconDirectory = appIconDirectory;

    public void UpdateAppInfoStaleThreshold(TimeSpan appInfoStaleThreshold)
    {
        if (appInfoStaleThreshold <= TimeSpan.Zero)
            throw new ArgumentException("App info stale threshold must be greater than zero.", nameof(appInfoStaleThreshold));
        AppInfoStaleThreshold = appInfoStaleThreshold;
    }

    public void UpdateActiveSessionAutoSaveInterval(TimeSpan activeSessionAutoSaveInterval)
    {
        if (activeSessionAutoSaveInterval <= TimeSpan.Zero)
            throw new ArgumentException("Active session auto save interval must be greater than zero.", nameof(activeSessionAutoSaveInterval));
        ActiveSessionAutoSaveInterval = activeSessionAutoSaveInterval;
    }

    public void UpdateIsIdleDetectionEnabled(bool isIdleDetectionEnabled) => IsIdleDetectionEnabled = isIdleDetectionEnabled;

    public void UpdateIdleThreshold(TimeSpan idleThreshold)
    {
        if (idleThreshold <= TimeSpan.Zero)
            throw new ArgumentException("Idle threshold must be greater than zero.", nameof(idleThreshold));
        IdleThreshold = idleThreshold;
    }

    public void UpdateIdleDetectionPollingInterval(TimeSpan idleDetectionPollingInterval)
    {
        if (idleDetectionPollingInterval <= TimeSpan.Zero)
            throw new ArgumentException("Idle detection polling interval must be greater than zero.", nameof(idleDetectionPollingInterval));
        IdleDetectionPollingInterval = idleDetectionPollingInterval;
    }

    public void UpdateMinValidSessionDuration(TimeSpan minValidSessionDuration)
    {
        if (minValidSessionDuration <= TimeSpan.Zero)
            throw new ArgumentException("Min valid session duration must be greater than zero.", nameof(minValidSessionDuration));
        MinValidSessionDuration = minValidSessionDuration;
    }

    public void UpdateSessionMergeTolerance(TimeSpan sessionMergeTolerance)
    {
        if (sessionMergeTolerance <= TimeSpan.Zero)
            throw new ArgumentException("Session merge tolerance must be greater than zero.", nameof(sessionMergeTolerance));
        SessionMergeTolerance = sessionMergeTolerance;
    }

    public void UpdateSessionOptimizationInterval(TimeSpan sessionOptimizationInterval)
    {
        if (sessionOptimizationInterval <= TimeSpan.Zero)
            throw new ArgumentException("Session optimization interval must be greater than zero.", nameof(sessionOptimizationInterval));
        SessionOptimizationInterval = sessionOptimizationInterval;
    }

    public void UpdateDayCutoffHour(int dayCutoffHour)
    {
        if (dayCutoffHour < 0 || dayCutoffHour > 23)
            throw new ArgumentException("Day cutoff hour must be between 0 and 23.", nameof(dayCutoffHour));
        DayCutoffHour = dayCutoffHour;
    }
}