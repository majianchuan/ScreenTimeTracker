using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageSummaryByHour;

public class GetUsageSummaryByHourEndpoint(
    IMediator mediator
    ) : Endpoint<GetUsageSummaryByHourRequest, List<GetUsageSummaryByHourResponseItem>>
{
    public override void Configure()
    {
        Get("usage/summary/hourly");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUsageSummaryByHourRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetUsageSummaryByHourQuery(
                req.Dimension,
                req.Date,
                req.ExcludedIds
            ),
            cancellationToken
        );
        await Send.OkAsync(result, cancellationToken);
    }
}
