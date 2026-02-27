namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageRankings;

public record GetUsageRankingsResponseItem(
    Guid Id,
    string Name,
    string? IconPath,
    long DurationSeconds,
    int Percentage
);