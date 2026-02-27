using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageByDay;

public record GetUsageByDayQuery(
    string Dimension,
    Guid Id,
    DateOnly StartDate,
    DateOnly EndDate
) : IRequest<List<GetUsageByDayResponseItem>>;
