namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageByHour;

public record GetUsageByHourResponseItem(
    int Hour,
    long DurationSeconds
);