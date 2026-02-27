using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserData.DeleteUsageData;

public record DeleteUsageDataRequest(
    [property: QueryParam, BindFrom("start-date")] DateOnly StartDate,
    [property: QueryParam, BindFrom("end-date")] DateOnly EndDate
);