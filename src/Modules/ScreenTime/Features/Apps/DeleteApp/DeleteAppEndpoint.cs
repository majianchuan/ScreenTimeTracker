using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.DeleteApp;

public class DeleteAppEndpoint(
    IMediator mediator
    ) : Endpoint<DeleteAppRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("apps/{Id}");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteAppRequest req, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new DeleteAppCommand(
                Id: req.Id
            ),
            cancellationToken
        );
        await Send.NoContentAsync(cancellationToken);
    }
}
