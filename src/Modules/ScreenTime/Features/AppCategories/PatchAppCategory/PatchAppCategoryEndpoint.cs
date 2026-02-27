using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.PatchAppCategory;

public class PatchAppCategoryEndpoint(
    IMediator mediator
    ) : Endpoint<PatchAppCategoryRequest, EmptyResponse>
{
    public override void Configure()
    {
        Patch("app-categories/{Id}");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatchAppCategoryRequest req, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new PatchAppCategoryCommand(
                Id: req.Id,
                Name: req.Name,
                IconPath: req.IconPath
            ),
            cancellationToken
        );
        await Send.NoContentAsync(cancellationToken);
    }
}
