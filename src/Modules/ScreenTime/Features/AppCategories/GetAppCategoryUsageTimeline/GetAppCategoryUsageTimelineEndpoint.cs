using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageTimeline.GetAppCategoryUsageTimeline;

public class GetAppCategoryUsageTimelineEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppCategoryUsageTimelineRequest, List<GetAppCategoryUsageTimelineResponseItem>>
{
    public override void Configure()
    {
        Get("usage/app-categories/timeline");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppCategoryUsageTimelineRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAppCategoryUsageTimelineQuery(
                req.StartDate,
                req.EndDate,
                req.ExcludedIds
            ),
            cancellationToken
        );

        await Send.OkAsync(result, cancellationToken);
    }
}
