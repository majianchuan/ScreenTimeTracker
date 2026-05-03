using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.DeleteUsageData;

public record DeleteUsageDataRequest(
    [property: QueryParam] DateOnly StartDate,
    [property: QueryParam] DateOnly EndDate
);