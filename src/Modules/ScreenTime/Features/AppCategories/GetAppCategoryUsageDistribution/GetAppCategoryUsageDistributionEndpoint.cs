using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryUsageDistribution;

public class GetAppCategoryUsageDistributionEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppCategoryUsageDistributionRequest, GetAppCategoryUsageDistributionResponse>
{
    public override void Configure()
    {
        Get("usage/app-categories/distribution");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppCategoryUsageDistributionRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAppCategoryUsageDistributionQuery(
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
