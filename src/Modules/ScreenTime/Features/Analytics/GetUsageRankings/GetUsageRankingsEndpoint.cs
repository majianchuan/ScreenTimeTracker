using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageRankings;

public class GetUsageRankingsEndpoint(
    IMediator mediator
    ) : Endpoint<GetUsageRankingsRequest, List<GetUsageRankingsResponseItem>>
{
    public override void Configure()
    {
        Get("usage/rankings");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUsageRankingsRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetUsageRankingsQuery(
                req.Dimension,
                req.StartDate,
                req.EndDate,
                req.TopN,
                req.ExcludedIds
            ),
            cancellationToken
        );
        await Send.OkAsync(result, cancellationToken);
    }
}
