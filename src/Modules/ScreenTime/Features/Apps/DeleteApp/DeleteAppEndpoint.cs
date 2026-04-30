using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.DeleteApp;

public class DeleteAppEndpoint(
    IMediator mediator
    ) : Endpoint<DeleteAppRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("apps/{appId}");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteAppRequest req, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new DeleteAppCommand(
                AppId: req.AppId
            ),
            cancellationToken
        );
        await Send.NoContentAsync(cancellationToken);
    }
}
