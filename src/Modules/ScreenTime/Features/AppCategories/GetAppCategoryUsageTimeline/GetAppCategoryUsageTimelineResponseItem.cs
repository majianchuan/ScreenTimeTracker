namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryUsageTimeline;

public record GetAppCategoryUsageTimelineResponseItem(
    Guid Id,
    string Name,
    string Color,
    string StartTime,
    string EndTime
);