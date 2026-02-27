namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageSummaryByHour;

public record GetUsageSummaryByHourResponseItem(
    int Hour,
    long DurationSeconds
);