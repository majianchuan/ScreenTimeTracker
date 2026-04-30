using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserData.DeleteUsageData;

public record DeleteUsageDataRequest(
    [property: QueryParam] DateOnly StartDate,
    [property: QueryParam] DateOnly EndDate
);