using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.DeleteApp;

public record DeleteAppCommand(
    Guid Id
) : IRequest;