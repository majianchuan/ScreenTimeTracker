using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageDistribution;

public class GetAppUsageDistributionEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppUsageDistributionRequest, GetAppUsageDistributionResponse>
{
    public override void Configure()
    {
        Get("usage/apps/distribution");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppUsageDistributionRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAppUsageDistributionQuery(
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
