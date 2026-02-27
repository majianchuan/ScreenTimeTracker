using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.ActivityLogs.RecordActivity;

public record RecordActivityCommand(TimeSpan Interval) : IRequest;