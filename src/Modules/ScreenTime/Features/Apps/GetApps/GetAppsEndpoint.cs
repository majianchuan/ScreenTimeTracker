using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetApps;

public class GetAppsEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppsRequest, List<Dictionary<string, object?>>>
{
    public override void Configure()
    {
        Get("apps");
        Group<ScreenTimeGroup>();
        AllowAnonymous();

        Description(d => d
            .Produces<List<GetAppsResponseItem>>(200, "application/json")
        );
    }

    public override async Task HandleAsync(GetAppsRequest req, CancellationToken cancellationToken)
    {
        var apps = await mediator.Send(
            new GetAppsQuery(
                req.Fields
            ),
            cancellationToken
        );
        await Send.OkAsync(apps, cancellationToken);
    }
}
