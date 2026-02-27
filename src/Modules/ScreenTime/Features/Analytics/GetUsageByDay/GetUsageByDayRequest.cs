using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageByDay;

public record GetUsageByDayRequest(
    [property: QueryParam] string Dimension,
    [property: QueryParam] Guid Id,
    [property: QueryParam, BindFrom("start-date")] DateOnly StartDate,
    [property: QueryParam, BindFrom("end-date")] DateOnly EndDate
);