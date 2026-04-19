using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageByDay;

public record GetUsageByDayRequest(
    [property: QueryParam] string Dimension,
    [property: QueryParam] Guid Id,
    [property: QueryParam, BindFrom("start-date")] DateOnly StartDate,
    [property: QueryParam, BindFrom("end-date")] DateOnly EndDate
);