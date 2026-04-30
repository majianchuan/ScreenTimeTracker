namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageRanking;

public record GetAppUsageRankingResponseItem(
    Guid Id,
    string Name,
    string? IconPath,
    long DurationSeconds,
    int Percentage
);