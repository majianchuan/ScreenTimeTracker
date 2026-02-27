using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetApps;

public record GetAppsRequest(
    [property: QueryParam] string? Fields
);