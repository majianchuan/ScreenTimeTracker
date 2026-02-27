namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageByDay;

public record GetUsageByDayResponseItem(
    DateOnly Date,
    long DurationSeconds
);