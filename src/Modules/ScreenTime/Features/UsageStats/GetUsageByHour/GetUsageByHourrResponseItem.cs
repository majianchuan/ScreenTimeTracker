namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageByHour;

public record GetUsageByHourResponseItem(
    int Hour,
    long DurationSeconds
);