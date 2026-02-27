using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageSummaryByDay;

public class GetUsageSummaryByDayEndpoint(
    IMediator mediator
    ) : Endpoint<GetUsageSummaryByDayRequest, List<GetUsageSummaryByDayResponseItem>>
{
    public override void Configure()
    {
        Get("usage/summary/daily");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUsageSummaryByDayRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetUsageSummaryByDayQuery(
                req.Dimension,
                req.StartDate,
                req.EndDate,
                req.ExcludedIds
            ),
            cancellationToken
        );
        await Send.OkAsync(result, cancellationToken);
    }
}
