using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageByHour;

public record GetUsageByHourQuery(
    string Dimension,
    Guid Id,
    DateOnly Date
) : IRequest<List<GetUsageByHourResponseItem>>;
