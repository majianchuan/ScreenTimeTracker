namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageSummaryByHour;

public record GetUsageSummaryByHourResponseItem(
    int Hour,
    long DurationSeconds
);