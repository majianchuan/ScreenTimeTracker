namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageTimeline;

public record GetAppUsageTimelineResponseItem(
    Guid Id,
    string Name,
    string Color,
    string StartTime,
    string EndTime
);