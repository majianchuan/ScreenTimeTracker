namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageTimeline;

public record GetAppUsageTimelineResponseItem(
    Guid Id,
    string Name,
    string Color,
    DateTime StartTime,
    DateTime EndTime
);