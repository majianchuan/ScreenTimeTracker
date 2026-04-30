using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageRanking.GetAppCategoryUsageRanking;

public class GetAppCategoryUsageRankingEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppCategoryUsageRankingRequest, List<GetAppCategoryUsageRankingResponseItem>>
{
    public override void Configure()
    {
        Get("usage/app-categories/ranking");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppCategoryUsageRankingRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAppCategoryUsageRankingQuery(
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
