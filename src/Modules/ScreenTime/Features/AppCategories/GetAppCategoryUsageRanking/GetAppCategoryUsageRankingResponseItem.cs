namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageRanking.GetAppCategoryUsageRanking;

public record GetAppCategoryUsageRankingResponseItem(
    Guid Id,
    string Name,
    string? IconPath,
    long DurationSeconds,
    int Percentage
);