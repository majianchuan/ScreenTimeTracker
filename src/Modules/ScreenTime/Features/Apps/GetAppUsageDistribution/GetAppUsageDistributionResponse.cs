namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageDistribution;

public record GetAppUsageDistributionResponse(
    List<AppUsageDistributionItem> Items,
    int TotalCount,
    long TotalDurationSeconds,
    int OthersCount,
    long OthersDurationSeconds
);

public record AppUsageDistributionItem(
    Guid Id,
    string Name,
    string Color,
    string? IconPath,
    DateTime IconLastUpdatedAt,
    long DurationSeconds
);