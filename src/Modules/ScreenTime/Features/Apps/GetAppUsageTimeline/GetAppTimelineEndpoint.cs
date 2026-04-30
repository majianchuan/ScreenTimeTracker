using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageTimeline;

public class GetAppUsageTimelineEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppUsageTimelineRequest, List<GetAppUsageTimelineResponseItem>>
{
    public override void Configure()
    {
        Get("usage/apps/timeline");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppUsageTimelineRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAppUsageTimelineQuery(
                req.StartDate,
                req.EndDate,
                req.ExcludedIds
            ),
            cancellationToken
        );
        await Send.OkAsync(result, cancellationToken);
    }
}
