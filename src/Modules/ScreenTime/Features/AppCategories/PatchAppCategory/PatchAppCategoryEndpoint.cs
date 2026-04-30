using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.PatchAppCategory;

public class PatchAppCategoryEndpoint(
    IMediator mediator
    ) : Endpoint<PatchAppCategoryRequest, EmptyResponse>
{
    public override void Configure()
    {
        Patch("app-categories/{appCategoryId}");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatchAppCategoryRequest req, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new PatchAppCategoryCommand(
                AppCategoryId: req.AppCategoryId,
                Name: req.Name,
                IconPath: req.IconPath
            ),
            cancellationToken
        );
        await Send.NoContentAsync(cancellationToken);
    }
}
