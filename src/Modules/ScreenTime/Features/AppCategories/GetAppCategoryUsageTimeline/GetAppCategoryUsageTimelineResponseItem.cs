namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryUsageTimeline;

public record GetAppCategoryUsageTimelineResponseItem(
    Guid Id,
    string Name,
    string Color,
    DateTime StartTime,
    DateTime EndTime
);