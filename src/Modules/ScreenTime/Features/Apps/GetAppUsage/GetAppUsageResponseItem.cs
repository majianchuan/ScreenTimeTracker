namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsage;

public record GetAppUsageResponseItem(
    DateTime StartTime,
    long DurationSeconds
);