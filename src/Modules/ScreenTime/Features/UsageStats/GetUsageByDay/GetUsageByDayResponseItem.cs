namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageByDay;

public record GetUsageByDayResponseItem(
    DateOnly Date,
    long DurationSeconds
);