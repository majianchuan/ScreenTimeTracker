using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetApp;

public class GetAppEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppRequest, GetAppResponse>
{
    public override void Configure()
    {
        Get("apps/{appId}");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppRequest req, CancellationToken cancellationToken)
    {
        var app = await mediator.Send(
            new GetAppQuery(req.AppId),
            cancellationToken
        );

        if (app is null)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        await Send.OkAsync(app, cancellationToken);
    }
}
