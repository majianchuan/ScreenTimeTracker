using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.PatchApp;

public class PatchAppEndpoint(
    IMediator mediator
    ) : Endpoint<PatchAppRequest, EmptyResponse>
{
    public override void Configure()
    {
        Patch("apps/{Id}");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatchAppRequest req, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new PatchAppCommand(
                Id: req.Id,
                Name: req.Name,
                IsAutoUpdateEnabled: req.IsAutoUpdateEnabled,
                AppCategoryId: req.AppCategoryId,
                IconPath: req.IconPath
            ),
            cancellationToken
        );
        await Send.NoContentAsync(cancellationToken);
    }
}
