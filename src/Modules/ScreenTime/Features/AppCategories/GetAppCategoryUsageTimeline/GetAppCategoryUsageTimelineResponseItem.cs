namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageTimeline.GetAppCategoryUsageTimeline;

public record GetAppCategoryUsageTimelineResponseItem(
    Guid Id,
    string Name,
    string StartTime,
    string EndTime
);