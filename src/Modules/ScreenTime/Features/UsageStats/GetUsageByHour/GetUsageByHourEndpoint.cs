using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageByHour;

public class GetUsageByHourEndpoint(
    IMediator mediator
    ) : Endpoint<GetUsageByHourRequest, List<GetUsageByHourResponseItem>>
{
    public override void Configure()
    {
        Get("usage/hourly");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUsageByHourRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetUsageByHourQuery(
                req.Dimension,
                req.Id,
                req.Date
            ),
            cancellationToken
        );
        await Send.OkAsync(result, cancellationToken);
    }
}
