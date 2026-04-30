namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryUsage;

public record GetAppCategoryUsageResponseItem(
    string StartTime,
    long DurationSeconds
);