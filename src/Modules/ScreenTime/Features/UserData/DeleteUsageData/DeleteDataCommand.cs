using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserData.DeleteUsageData;

public record DeleteUsageDataCommand(
    DateOnly StartDate,
    DateOnly EndDate
) : IRequest;