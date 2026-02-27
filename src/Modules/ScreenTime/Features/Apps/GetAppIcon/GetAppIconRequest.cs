using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppIcon;

public record GetAppIconRequest(
    [property: RouteParam] Guid Id
);