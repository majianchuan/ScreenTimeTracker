namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageSummaryByDay;

public record GetUsageSummaryByDayResponseItem(
    DateOnly Date,
    long DurationSeconds
);