namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageSummaryByDay;

public record GetUsageSummaryByDayResponseItem(
    DateOnly Date,
    long DurationSeconds
);