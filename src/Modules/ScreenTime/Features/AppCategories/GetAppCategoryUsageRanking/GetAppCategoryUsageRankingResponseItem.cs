namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryUsageRanking;

public record GetAppCategoryUsageRankingResponseItem(
    Guid Id,
    string Name,
    string Color,
    string? IconPath,
    long DurationSeconds,
    int Percentage
);