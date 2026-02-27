using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageByHour;

public record GetUsageByHourRequest(
    [property: QueryParam] string Dimension,
    [property: QueryParam] Guid Id,
    [property: QueryParam] DateOnly Date
);