using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.DeleteUsageData;

public record DeleteUsageDataCommand(
    DateOnly StartDate,
    DateOnly EndDate,
    TimeSpan MinDuration = default
) : IRequest;