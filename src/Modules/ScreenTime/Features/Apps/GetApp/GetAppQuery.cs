using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetApp;

public record GetAppQuery(
    Guid AppId
) : IRequest<GetAppResponse?>;
