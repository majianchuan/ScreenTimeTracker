using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetApp;

public record GetAppRequest(
    [property: RouteParam] Guid AppId
);