using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageByDay;

public class GetUsageByDayEndpoint(
    IMediator mediator
    ) : Endpoint<GetUsageByDayRequest, List<GetUsageByDayResponseItem>>
{
    public override void Configure()
    {
        Get("usage/daily");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUsageByDayRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetUsageByDayQuery(
                req.Dimension,
                req.Id,
                req.StartDate,
                req.EndDate
            ),
            cancellationToken
        );
        await Send.OkAsync(result, cancellationToken);
    }
}
