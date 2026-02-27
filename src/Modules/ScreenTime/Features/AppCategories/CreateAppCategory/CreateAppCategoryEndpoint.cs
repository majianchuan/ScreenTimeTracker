using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.CreateAppCategory;

public class CreateAppCategoryEndpoint(
    IMediator mediator
    ) : Endpoint<CreateAppCategoryRequest, CreateAppCategoryResponse>
{
    public override void Configure()
    {
        Post("app-categories");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateAppCategoryRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateAppCategoryCommand(
                req.Name,
                req.IconPath
            ),
            cancellationToken
        );
        await Send.OkAsync(result, cancellationToken);
    }
}
