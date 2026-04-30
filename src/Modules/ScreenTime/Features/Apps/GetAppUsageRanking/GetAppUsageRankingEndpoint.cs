using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageRanking;

public class GetAppUsageRankingEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppUsageRankingRequest, List<GetAppUsageRankingResponseItem>>
{
    public override void Configure()
    {
        Get("usage/apps/ranking");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppUsageRankingRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAppUsageRankingQuery(
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
