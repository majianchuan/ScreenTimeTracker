using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsage;

public class GetAppUsageEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppUsageRequest, List<GetAppUsageResponseItem>>
{
    public override void Configure()
    {
        Get("usage/apps");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppUsageRequest req, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new GetAppUsageQuery(
                req.Granularity,
                req.StartDate,
                req.EndDate,
                req.IncludedIds,
                req.ExcludedIds
            ),
            cancellationToken
        );
        await Send.OkAsync(response, cancellationToken);
    }
}
