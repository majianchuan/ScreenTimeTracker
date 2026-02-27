using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppIcon;

public record GetAppIconPathQuery(
    Guid Id
) : IRequest<string?>;
