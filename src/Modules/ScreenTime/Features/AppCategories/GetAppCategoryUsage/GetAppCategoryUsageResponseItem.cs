namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryUsage;

public record GetAppCategoryUsageResponseItem(
    DateTime StartTime,
    long DurationSeconds
);