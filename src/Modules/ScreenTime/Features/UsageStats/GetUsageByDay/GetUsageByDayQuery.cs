using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageByDay;

public record GetUsageByDayQuery(
    string Dimension,
    Guid Id,
    DateOnly StartDate,
    DateOnly EndDate
) : IRequest<List<GetUsageByDayResponseItem>>;
