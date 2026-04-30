namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsage;

public record GetAppUsageResponseItem(
    string StartTime,
    long DurationSeconds
);